using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiOperationFilter
    {
        void Apply(OpenApiOperation operation, IOpenApiOperationFilterContext operationFilterContext);
    }
}