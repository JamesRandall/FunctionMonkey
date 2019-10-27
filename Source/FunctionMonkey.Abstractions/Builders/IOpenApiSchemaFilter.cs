using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.OpenApi
{
    public interface IOpenApiSchemaFilter
    {
        void Apply(OpenApiSchema schema, IOpenApiSchemaFilterContext schemaFilterContext);
    }
}