using FunctionMonkey.Abstractions.Builders;
using System.Collections.Generic;
using System.Net.Http;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    public class OpenApiHttpFunctionFilterDummy : IOpenApiHttpFunctionFilter
    {
        public void Apply(string route, ISet<HttpMethod> verbs, IOpenApiHttpFunctionFilterContext functionFilterContext)
        {
        }
    }
}
