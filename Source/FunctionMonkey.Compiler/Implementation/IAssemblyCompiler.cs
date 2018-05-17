using System.Collections.Generic;
using System.Reflection;
using FunctionMonkey.Model;

namespace FunctionMonkey.Compiler.Implementation
{
    internal interface IAssemblyCompiler
    {
        void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            IReadOnlyCollection<Assembly> externalAssemblies,
            string outputBinaryFolder,
            string assemblyName, bool openApiEndpointRequired, string outputAuthoredSourceFolder = null);
    }
}
