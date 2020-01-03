using System;
using System.Collections.Generic;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    class OpenApiSchemaFilterContext : IOpenApiSchemaFilterContext
    {
        public Type Type { get; set; }

        public Dictionary<string, string> PropertyNames { get; set; }
    }
}
