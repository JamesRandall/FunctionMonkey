using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiParameterFilter
    {
        void Apply(OpenApiParameter parameter, IOpenApiParameterFilterContext parameterFilterContext);
    }
}