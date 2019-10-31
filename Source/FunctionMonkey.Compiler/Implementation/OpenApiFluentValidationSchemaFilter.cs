using FluentValidation;
using FunctionMonkey.Abstractions.OpenApi;
using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Compiler.Implementation
{
    class OpenApiFluentValidationSchemaFilter : IOpenApiSchemaFilter
    {
        private readonly IValidatorFactory _validatorFactory;

        public OpenApiFluentValidationSchemaFilter(IValidatorFactory validatorFactory = null)
        {
            _validatorFactory = validatorFactory;
        }

        public void Apply(OpenApiSchema schema, IOpenApiSchemaFilterContext schemaFilterContext)
        {
            throw new System.NotImplementedException();
        }
    }
}
