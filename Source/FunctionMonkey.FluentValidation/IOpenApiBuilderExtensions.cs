using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation.OpenApi;
using System.Collections.Generic;
using System.Reflection;

namespace FunctionMonkey.FluentValidation
{
    public static class IOpenApiBuilderExtensions
    {
        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on FluentValidation. 
        /// </summary>
        /// <param name="openApiBuilder">The OpenApiBuilder instance</param>
        /// <param name="assembly">The assembly to search for FluentValidators</param>
        /// <param name="customRules">Custom rules to overwrite some or all standard rules</param>
        public static IOpenApiBuilder AddValidatorsFromAssembly(this IOpenApiBuilder openApiBuilder,
            Assembly assembly,
            IEnumerable<OpenApiFluentValidationRule> customRules = null)
        {
            openApiBuilder.AddSchemaFilter(() => new OpenApiFluentValidationSchemaFilter(assembly, customRules));
            return openApiBuilder;
        }
    }
}
