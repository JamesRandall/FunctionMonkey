using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiHttpFunctionFilter
    {
        void Apply(string route, ISet<HttpMethod> verbs, IOpenApiHttpFunctionFilterContext functionFilterContext);
    }
}