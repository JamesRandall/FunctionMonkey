using FunctionMonkey.Abstractions.Builders;
using System;
using System.Collections.Generic;

namespace FunctionMonkey.Compiler.Implementation
{
    class OpenApiSchemaFilterContext : IOpenApiSchemaFilterContext
    {
        public Type Type { get; set; }

        public Dictionary<string, string> PropertyNames { get; set; }
    }
}
