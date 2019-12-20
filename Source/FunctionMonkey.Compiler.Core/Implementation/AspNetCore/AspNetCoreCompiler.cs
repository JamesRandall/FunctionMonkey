using System.Collections.Generic;
using FunctionMonkey.Abstractions;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal class AspNetCoreCompiler : ITargetCompiler
    {
        private readonly ICompilerLog _compilerLog;

        public AspNetCoreCompiler(ICompilerLog compilerLog)
        {
            _compilerLog = compilerLog;
        }
        
        public bool CompileAssets(IFunctionCompilerMetadata functionCompilerMetadata, string newAssemblyNamespace,
            IFunctionAppConfiguration configuration, IReadOnlyCollection<string> externalAssemblies, string outputBinaryFolder)
        {
            throw new System.NotImplementedException();
        }
    }
}