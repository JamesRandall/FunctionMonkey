using FluentValidation.Validators;
using System.Collections.Generic;

namespace FunctionMonkey.FluentValidation.OpenApi
{
    class OpenApiFluentValidationDefaultRulesFactory
    {
        /// <summary>
        /// Creates default rules.
        /// Can be overriden by name.
        /// </summary>
        public static IReadOnlyList<OpenApiFluentValidationRule> CreateDefaultRules()
        {

            var defaultRules = new List<OpenApiFluentValidationRule>
            {
                new OpenApiFluentValidationRule("Required")
                {
                    Matches = propertyValidator =>
                        propertyValidator is INotNullValidator || propertyValidator is INotEmptyValidator,
                    Apply = context =>
                    {
                        if (!context.Schema.Required.Contains(context.PropertyKey))
                        {
                            context.Schema.Required.Add(context.PropertyKey);
                        }
                    }
                },
                new OpenApiFluentValidationRule("NotEmpty")
                {
                    Matches = propertyValidator => propertyValidator is INotEmptyValidator,
                    Apply = context => { context.Schema.Properties[context.PropertyKey].MinLength = 1; }
                },
                new OpenApiFluentValidationRule("Length")
                {
                    Matches = propertyValidator => propertyValidator is ILengthValidator,
                    Apply = context =>
                    {
                        var lengthValidator = (ILengthValidator) context.PropertyValidator;

                        if (lengthValidator.Max > 0)
                        {
                            context.Schema.Properties[context.PropertyKey].MaxLength = lengthValidator.Max;
                        }

                        if (lengthValidator is MinimumLengthValidator
                            || lengthValidator is ExactLengthValidator
                            || context.Schema.Properties[context.PropertyKey].MinLength == null)
                        {
                            context.Schema.Properties[context.PropertyKey].MinLength = lengthValidator.Min;
                        }
                    }
                },
                new OpenApiFluentValidationRule("Pattern")
                {
                    Matches = propertyValidator => propertyValidator is IRegularExpressionValidator,
                    Apply = context =>
                    {
                        var regularExpressionValidator = (IRegularExpressionValidator) context.PropertyValidator;
                        context.Schema.Properties[context.PropertyKey].Pattern = regularExpressionValidator.Expression;
                    }
                },
                new OpenApiFluentValidationRule("Comparison")
                {
                    Matches = propertyValidator => propertyValidator is IComparisonValidator,
                    Apply = context =>
                    {
                        var comparisonValidator = (IComparisonValidator) context.PropertyValidator;
                        if (comparisonValidator.ValueToCompare.IsNumeric())
                        {
                            var valueToCompare = comparisonValidator.ValueToCompare.NumericToDecimal();
                            var schemaProperty = context.Schema.Properties[context.PropertyKey];

                            if (comparisonValidator.Comparison == Comparison.GreaterThanOrEqual)
                            {
                                schemaProperty.Minimum = valueToCompare;
                            }
                            else if (comparisonValidator.Comparison == Comparison.GreaterThan)
                            {
                                schemaProperty.Minimum = valueToCompare;
                                schemaProperty.ExclusiveMinimum = true;
                            }
                            else if (comparisonValidator.Comparison == Comparison.LessThanOrEqual)
                            {
                                schemaProperty.Maximum = valueToCompare;
                            }
                            else if (comparisonValidator.Comparison == Comparison.LessThan)
                            {
                                schemaProperty.Maximum = valueToCompare;
                                schemaProperty.ExclusiveMaximum = true;
                            }
                        }
                    }
                },
                new OpenApiFluentValidationRule("Between")
                {
                    Matches = propertyValidator => propertyValidator is IBetweenValidator,
                    Apply = context =>
                    {
                        var betweenValidator = (IBetweenValidator) context.PropertyValidator;
                        var schemaProperty = context.Schema.Properties[context.PropertyKey];

                        if (betweenValidator.From.IsNumeric())
                        {
                            schemaProperty.Minimum = betweenValidator.From.NumericToDecimal();

                            if (betweenValidator is ExclusiveBetweenValidator)
                            {
                                schemaProperty.ExclusiveMinimum = true;
                            }
                        }

                        if (betweenValidator.To.IsNumeric())
                        {
                            schemaProperty.Maximum = betweenValidator.To.NumericToDecimal();

                            if (betweenValidator is ExclusiveBetweenValidator)
                            {
                                schemaProperty.ExclusiveMaximum = true;
                            }
                        }
                    }
                }
            };

            return defaultRules;
        }
    }
}
