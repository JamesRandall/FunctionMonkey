using System;
using System.Collections.Generic;
using System.Reflection;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal interface IAssemblyCompiler
    {
        void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            IReadOnlyCollection<string> externalAssemblyLocations,
            string outputBinaryFolder,
            string assemblyName,
            OpenApiOutputModel openApiOutputModel,
            CompileTargetEnum compileTarget,
            string outputAuthoredSourceFolder = null);
    }
}
