using System.Collections.Generic;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal interface ITargetCompiler
    {
        bool CompileAssets(IFunctionCompilerMetadata functionCompilerMetadata,
            string newAssemblyNamespace,
            IFunctionAppConfiguration configuration, IReadOnlyCollection<string> externalAssemblies,
            string outputBinaryFolder);
    }
}