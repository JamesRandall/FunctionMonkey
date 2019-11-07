using System;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Compiler.Implementation
{
    public class OpenApiOperationFilterContext : IOpenApiOperationFilterContext
    {
        public Type CommandType { get; set; }
    }
}
