using System.Collections.Generic;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;

namespace FunctionMonkey.Compiler.Core.Implementation.AspNetCore
{
    internal class AspNetCoreCompiler : ITargetCompiler
    {
        private readonly ICompilerLog _compilerLog;
        private readonly IAssemblyCompiler _assemblyCompiler;
        private readonly OpenApiCompiler _openApiCompiler;

        public AspNetCoreCompiler(ICompilerLog compilerLog)
        {
            _compilerLog = compilerLog;
            _assemblyCompiler = new AspNetCoreAssemblyCompiler(compilerLog, new TemplateProvider(CompileTargetEnum.AspNetCore));
            _openApiCompiler = new OpenApiCompiler();
        }
        
        public bool CompileAssets(IFunctionCompilerMetadata functionCompilerMetadata, string newAssemblyNamespace,
            IFunctionAppConfiguration configuration, IReadOnlyCollection<string> externalAssemblies, string outputBinaryFolder)
        {
            _compilerLog.Warning("ASP.Net Core output is currently experimental");
            
            FunctionMonkey.Compiler.Core.HandlebarsHelpers.AspNetCore.HandlebarsHelperRegistration.RegisterHelpers();
            
            OpenApiOutputModel openApi = _openApiCompiler.Compile(functionCompilerMetadata.OpenApiConfiguration,
                functionCompilerMetadata.FunctionDefinitions, outputBinaryFolder);
            _assemblyCompiler.OpenApiOutputModel = openApi;
            
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