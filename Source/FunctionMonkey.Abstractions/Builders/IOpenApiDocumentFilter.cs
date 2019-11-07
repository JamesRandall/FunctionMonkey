using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiDocumentFilter
    {
        void Apply(OpenApiDocument document, IOpenApiDocumentFilterContext documentFilterContext);
    }
}