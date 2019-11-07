using System;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiOperationFilterContext
    {
        Type CommandType { get; set; }
    }
}