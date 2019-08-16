using System.Collections.Generic;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;

namespace FunctionMonkey.Compiler.Implementation
{
    internal class FunctionCompilerMetadata : IFunctionCompilerMetadata
    {
        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; set;  }
        public OpenApiConfiguration OpenApiConfiguration { get; set; }
        public string OutputAuthoredSourceFolder { get; set; }
        
        public IReadOnlyCollection<AbstractClaimsMappingDefinition> ClaimsMappings { get; }
    }
}