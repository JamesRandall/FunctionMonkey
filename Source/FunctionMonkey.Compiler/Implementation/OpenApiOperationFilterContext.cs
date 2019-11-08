using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace FunctionMonkey.Compiler.Implementation
{
    public class OpenApiOperationFilterContext : IOpenApiOperationFilterContext
    {
        public Type CommandType { get; set; }

        public Dictionary<string, string> PropertyNames { get; set; }
    }
}
