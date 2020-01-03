using System;
using System.Collections.Generic;
using System.Reflection;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal interface IAssemblyCompiler
    {
        bool Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            IReadOnlyCollection<string> externalAssemblyLocations,
            string outputBinaryFolder,
            string assemblyName,
            CompileTargetEnum compileTarget,
            string outputAuthoredSourceFolder = null);
        
        OpenApiOutputModel OpenApiOutputModel { get; set; }
    }
}
