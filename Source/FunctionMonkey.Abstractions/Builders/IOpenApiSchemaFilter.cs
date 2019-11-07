using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiSchemaFilter
    {
        void Apply(OpenApiSchema schema, IOpenApiSchemaFilterContext schemaFilterContext);
    }
}