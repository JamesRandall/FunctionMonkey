using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.OpenApi
{
    public interface IOpenApiDocumentFilter
    {
        void Apply(OpenApiDocument document, IOpenApiDocumentFilterContext documentFilterContext);
    }
}