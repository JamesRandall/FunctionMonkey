using System;
using System.Collections.Generic;
using System.Reflection;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;
using FunctionMonkey.Model;

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
            CompilerOptions compilerOptions,
            string outputAuthoredSourceFolder = null);
        
        OpenApiOutputModel OpenApiOutputModel { get; set; }
    }
}
