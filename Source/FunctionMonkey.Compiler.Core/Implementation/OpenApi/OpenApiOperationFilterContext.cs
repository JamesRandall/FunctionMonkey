using System;
using System.Collections.Generic;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    public class OpenApiOperationFilterContext : IOpenApiOperationFilterContext
    {
        public Type CommandType { get; set; }

        public Dictionary<string, string> PropertyNames { get; set; }
    }
}
