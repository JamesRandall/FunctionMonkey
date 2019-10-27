using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.OpenApi
{
    public interface IOpenApiParameterFilter
    {
        void Apply(OpenApiParameter parameter, IOpenApiParameterFilterContext parameterFilterContext);
    }
}