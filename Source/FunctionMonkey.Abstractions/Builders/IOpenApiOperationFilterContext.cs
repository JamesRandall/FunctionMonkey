using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IOpenApiOperationFilterContext
    {
        Type CommandType { get; set; }

        Dictionary<string, string> PropertyNames { get; set; }
    }
}