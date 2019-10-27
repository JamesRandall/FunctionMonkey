using FunctionMonkey.Abstractions.OpenApi;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace FunctionMonkey.Compiler.Implementation
{
    class OpenApiSchemaFilterContext : IOpenApiSchemaFilterContext
    {
        public Type Type { get; set; }

        public Dictionary<string, string> PropertyNames { get; set; }
    }
}
