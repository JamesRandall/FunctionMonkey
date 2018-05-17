using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation
{
    internal interface IAssemblyCompiler
    {
        void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            IReadOnlyCollection<Assembly> externalAssemblies,
            string outputBinaryFolder,
            string assemblyName, bool openApiEndpointRequired, string outputAuthoredSourceFolder = null);
    }
}
