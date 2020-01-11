using System.Collections.Generic;
using System.Linq;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;

namespace FunctionMonkey.Compiler.Core.Implementation.AzureFunctions
{
    internal class AzureFunctionsCompiler : ITargetCompiler
    {
        private readonly ICompilerLog _compilerLog;
        private readonly JsonCompiler _jsonCompiler;
        private readonly OpenApiCompiler _openApiCompiler;
        private readonly IAssemblyCompiler _assemblyCompiler;
        
        public AzureFunctionsCompiler(ICompilerLog compilerLog)
        {
            _compilerLog = compilerLog;
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
            HandlebarsHelpers.AzureFunctions.HandlebarsHelperRegistration.RegisterHelpers();
            
            bool isFSharpProject = functionCompilerMetadata.FunctionDefinitions.Any(x => x.IsFunctionalFunction);
            if (isFSharpProject)
            {
                _compilerLog.Warning("FSharp output is currently experimental");
            }

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
                functionCompilerMetadata.CompilerOptions,
                functionCompilerMetadata.OutputAuthoredSourceFolder);
        }
    }
}
