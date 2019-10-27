using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.OpenApi
{
    public interface IOpenApiOperationFilter
    {
        void Apply(OpenApiOperation operation, IOpenApiOperationFilterContext operationFilterContext);
    }
}