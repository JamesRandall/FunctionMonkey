using FluentValidation.Validators;
using Microsoft.OpenApi.Models;

namespace FunctionMonkey.FluentValidation.OpenApi
{
    public class OpenApiFluentValidationRuleContext
    {
        public OpenApiSchema Schema { get; }

        public string PropertyKey { get; }

        public IPropertyValidator PropertyValidator { get; }

        public OpenApiFluentValidationRuleContext(OpenApiSchema schema,
            string propertyKey,
            IPropertyValidator propertyValidator)
        {
            Schema = schema;
            PropertyKey = propertyKey;
            PropertyValidator = propertyValidator;
        }
    }
}
