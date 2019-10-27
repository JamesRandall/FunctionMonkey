using System;
using System.Collections.Generic;
using System.Reflection;

namespace FunctionMonkey.Abstractions.OpenApi
{
    public interface IOpenApiSchemaFilterContext
    {
        Type Type { get; set; }

        Dictionary<string, string> PropertyNames { get; set; }
    }
}