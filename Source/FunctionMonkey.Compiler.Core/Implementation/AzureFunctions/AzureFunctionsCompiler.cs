using FunctionMonkey.Abstractions;
using FunctionMonkey.Compiler.Core.Implementation;
using System.Collections.Generic;

namespace FunctionMonkey.Compiler.Core
{
    internal class AzureFunctionsCompiler : ITargetCompiler
    {
        private readonly JsonCompiler _jsonCompiler;
        private readonly OpenApiCompiler _openApiCompiler;
        private readonly AzureFunctionsAssemblyCompiler _assemblyCompiler;
        
        public AzureFunctionsCompiler(ICompilerLog compilerLog)
        {
            _jsonCompiler = new JsonCompiler();
            _openApiCompiler = new OpenApiCompiler();
            _assemblyCompiler = new AzureFunctionsAssemblyCompiler(compilerLog, new TemplateProvider(CompileTargetEnum.AzureFunctions));
        }

        public bool CompileAssets(IFunctionCompilerMetadata functionCompilerMetadata,
            string newAssemblyNamespace,
            IFunctionAppConfiguration configuration,
            IReadOnlyCollection<string> externalAssemblies,
            string outputBinaryFolder)
        {
            OpenApiOutputModel openApi = _openApiCompiler.Compile(functionCompilerMetadata.OpenApiConfiguration,
                functionCompilerMetadata.FunctionDefinitions, outputBinaryFolder);
            _assemblyCompiler.OpenApiOutputModel = openApi;

            _jsonCompiler.Compile(functionCompilerMetadata.FunctionDefinitions, openApi, outputBinaryFolder,
                newAssemblyNamespace);

            return _assemblyCompiler.Compile(functionCompilerMetadata.FunctionDefinitions,
                configuration?.GetType() ?? functionCompilerMetadata.BacklinkReferenceType,
                configuration != null ? null : functionCompilerMetadata.BacklinkPropertyInfo,
                newAssemblyNamespace,
                externalAssemblies,
                outputBinaryFolder,
                $"{newAssemblyNamespace}.dll",
                functionCompilerMetadata.CompileTarget,
                functionCompilerMetadata.OutputAuthoredSourceFolder);
        }
    }
}
