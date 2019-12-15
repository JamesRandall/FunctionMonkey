using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Validators;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FunctionMonkey.FluentValidation.OpenApi
{
    class OpenApiFluentValidationSchemaFilter : IOpenApiSchemaFilter
    {
        private readonly IDictionary<Type, Type> _validatorTypes = new Dictionary<Type, Type>();

        private readonly IReadOnlyList<OpenApiFluentValidationRule> _rules;

        public OpenApiFluentValidationSchemaFilter(Assembly assembly, IEnumerable<OpenApiFluentValidationRule> customRules)
        {
            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(scanResult =>
                {
                    var interfaceType = scanResult.InterfaceType;
                    if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IValidator<>))
                    {
                        _validatorTypes[interfaceType.GetGenericArguments()[0]] = scanResult.ValidatorType;
                    }
                });

            _rules = OpenApiFluentValidationDefaultRulesFactory.CreateDefaultRules();
            if (customRules != null)
            {
                var ruleMap = _rules.ToDictionary(rule => rule.Name, rule => rule);
                foreach (var rule in customRules)
                {
                    ruleMap[rule.Name] = rule;
                }
                _rules = ruleMap.Values.ToList();
            }
        }

        public void Apply(OpenApiSchema schema, IOpenApiSchemaFilterContext schemaFilterContext)
        {
            _validatorTypes.TryGetValue(schemaFilterContext.Type, out var validatorType);
            if (validatorType == null || validatorType.GetConstructor(Type.EmptyTypes) == null)
            {
                return;
            }

            var validator = (IValidator)Activator.CreateInstance(validatorType);
            ApplyRulesToSchema(schema, schemaFilterContext, validator);
            AddRulesFromIncludedValidators(schema, schemaFilterContext, validator);
        }

        private void ApplyRulesToSchema(OpenApiSchema schema, IOpenApiSchemaFilterContext schemaFilterContext, IValidator validator)
        {
            foreach (var key in schema?.Properties?.Keys ?? Array.Empty<string>())
            {
                var propertyName = schemaFilterContext.PropertyNames[key];
                var validators = validator.GetValidatorsForMember(propertyName);

                foreach (var propertyValidator in validators)
                {
                    foreach (var rule in _rules)
                    {
                        if (rule.Matches(propertyValidator))
                        {
                            rule.Apply(new OpenApiFluentValidationRuleContext(schema, key, propertyValidator));
                        }
                    }
                }
            }
        }

        private void AddRulesFromIncludedValidators(OpenApiSchema schema, IOpenApiSchemaFilterContext schemaFilterContext, IValidator validator)
        {
            var childAdapters = (validator as IEnumerable<IValidationRule>)
                .NotNull()
                .OfType<IncludeRule>()
                .Where(includeRule => includeRule.Condition == null && includeRule.AsyncCondition == null)
                .SelectMany(includeRule => includeRule.Validators)
                .OfType<ChildValidatorAdaptor>();

            foreach (var adapter in childAdapters)
            {
                var propertyValidatorContext = new PropertyValidatorContext(new ValidationContext(null), null, string.Empty);
                var includeValidator = adapter.GetValidator(propertyValidatorContext);
                ApplyRulesToSchema(schema, schemaFilterContext, includeValidator);
                AddRulesFromIncludedValidators(schema, schemaFilterContext, includeValidator);
            }
        }
    }
}
