using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi.Models;

namespace FunctionMonkey.FluentValidation.OpenApi
{
    class OpenApiFluentValidationOperationFilter : IOpenApiOperationFilter
    {
        private readonly IDictionary<Type, Type> _validatorTypes = new Dictionary<Type, Type>();

        private readonly IReadOnlyList<OpenApiFluentValidationRule> _rules;

        public OpenApiFluentValidationOperationFilter(Assembly assembly, IEnumerable<OpenApiFluentValidationRule> customRules)
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

        public void Apply(OpenApiOperation operation, IOpenApiOperationFilterContext operationFilterContext)
        {
            ApplyInternal(operation, operationFilterContext);
        }

        private void ApplyInternal(OpenApiOperation operation, IOpenApiOperationFilterContext operationFilterContext)
        {
            var parameterType = operationFilterContext.CommandType;

            if (operation.Parameters == null)
            {
                return;
            }

            //var schemaIdSelector = _swaggerGenOptions.SchemaRegistryOptions.SchemaIdSelector ?? new SchemaRegistryOptions().SchemaIdSelector;

            foreach (var operationParameter in operation.Parameters)
            {
                int i = 0;
                /*
                var apiParameterDescription = operationFilterContext.ApiDescription.ParameterDescriptions.FirstOrDefault(description =>
                    description.Name.Equals(operationParameter.Name, StringComparison.InvariantCultureIgnoreCase));

                var modelMetadata = apiParameterDescription?.ModelMetadata;
                if (modelMetadata != null)
                {
                    var parameterType = modelMetadata.ContainerType;
                    if (parameterType == null)
                    {
                        continue;
                    }

                    var validator = _validatorFactory.GetValidator(parameterType);
                    if (validator == null)
                    {
                        continue;
                    }

                    var key = modelMetadata.PropertyName;
                    var validatorsForMember = validator.GetValidatorsForMemberIgnoreCase(key);

                    var lazyLog = new LazyLog(_logger,
                        logger => logger.LogDebug($"Applying FluentValidation rules to swagger schema for type '{parameterType}' from operation '{operation.OperationId}'."));

                    Schema schema = null;
                    foreach (var propertyValidator in validatorsForMember)
                    {
                        foreach (var rule in _rules)
                        {
                            if (rule.Matches(propertyValidator))
                            {
                                try
                                {
                                    var schemaId = schemaIdSelector(parameterType);

                                    if (!context.SchemaRegistry.Definitions.TryGetValue(schemaId, out schema))
                                        schema = context.SchemaRegistry.GetOrRegister(parameterType);

                                    if (schema.Properties == null && context.SchemaRegistry.Definitions.ContainsKey(schemaId))
                                        schema = context.SchemaRegistry.Definitions[schemaId];

                                    if (schema.Properties != null && schema.Properties.Count > 0)
                                    {
                                        lazyLog.LogOnce();
                                        var schemaFilterContext = new SchemaFilterContext(parameterType, null, context.SchemaRegistry);

                                        // try to fix property casing (between property name and schema property name)
                                        var schemaProperty = schema.Properties.Keys.FirstOrDefault(k => string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
                                        if (schemaProperty != null)
                                            key = schemaProperty;

                                        rule.Apply(new RuleContext(schema, schemaFilterContext, key, propertyValidator));
                                        _logger.LogDebug($"Rule '{rule.Name}' applied for property '{parameterType.Name}.{key}'.");
                                    }
                                    else
                                    {
                                        _logger.LogDebug($"Rule '{rule.Name}' skipped for property '{parameterType.Name}.{key}'.");
                                    }
                                }
                                catch (Exception e)
                                {
                                    _logger.LogWarning(0, e, $"Error on apply rule '{rule.Name}' for property '{parameterType.Name}.{key}'.");
                                }
                            }
                        }
                    }

                    if (schema?.Required != null)
                        operationParameter.Required = schema.Required.Contains(key, StringComparer.InvariantCultureIgnoreCase);

                    if (schema?.Properties != null)
                    {
                        var parameterSchema = operationParameter as PartialSchema;
                        if (operationParameter != null)
                        {
                            if (schema.Properties.TryGetValue(key.ToLowerCamelCase(), out var property)
                                || schema.Properties.TryGetValue(key, out property))
                            {
                                parameterSchema.MinLength = property.MinLength;
                                parameterSchema.MaxLength = property.MaxLength;
                                parameterSchema.Pattern = property.Pattern;
                                parameterSchema.Minimum = property.Minimum;
                                parameterSchema.Maximum = property.Maximum;
                                parameterSchema.ExclusiveMaximum = property.ExclusiveMaximum;
                                parameterSchema.ExclusiveMinimum = property.ExclusiveMinimum;
                            }
                        }
                    }
                }
            */
            }
        }
    }
}
