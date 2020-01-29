using FunctionMonkey.Abstractions.Builders;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace OpenApi
{
    public class CustomOpenApiHttpFunctionFilter : IOpenApiHttpFunctionFilter
    {
        public void Apply(string route, ISet<HttpMethod> verbs, IOpenApiHttpFunctionFilterContext functionFilterContext)
        {
            verbs.Remove(HttpMethod.Post);
        }
    }
}
