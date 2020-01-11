using System;
using System.Collections.Generic;
using System.Reflection;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.Model;

namespace FunctionMonkey.Abstractions
{
    public interface IFunctionCompilerMetadata
    {
        IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; }
            
        OpenApiConfiguration OpenApiConfiguration { get; }
            
        string OutputAuthoredSourceFolder { get; }
        
        IReadOnlyCollection<AbstractClaimsMappingDefinition> ClaimsMappings { get; }
        
        Type BacklinkReferenceType { get; }

        PropertyInfo BacklinkPropertyInfo { get; }
        
        CompilerOptions CompilerOptions { get; }
    }
}