using System.Collections.Generic;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;

namespace FunctionMonkey.Abstractions
{
    public interface IFunctionCompilerMetadata
    {
        IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; }
            
        OpenApiConfiguration OpenApiConfiguration { get; }
            
        string OutputAuthoredSourceFolder { get; }
    }
}