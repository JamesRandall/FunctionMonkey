using System;
using System.Collections.Generic;
using System.Reflection;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Model;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal class FunctionCompilerMetadata : IFunctionCompilerMetadata
    {
        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; set;  }
        public OpenApiConfiguration OpenApiConfiguration { get; set; }
        public string OutputAuthoredSourceFolder { get; set; }
        public Type BacklinkReferenceType { get; set; }
        public PropertyInfo BacklinkPropertyInfo { get; }
        public CompilerOptions CompilerOptions { get; set; }
        public IReadOnlyCollection<AbstractClaimsMappingDefinition> ClaimsMappings { get; }
    }
}