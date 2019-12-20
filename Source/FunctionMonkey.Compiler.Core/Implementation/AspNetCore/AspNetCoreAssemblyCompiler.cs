using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using FunctionMonkey.Abstractions.Builders.Model;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal class AspNetCoreAssemblyCompiler : AssemblyCompilerBase
    {
        public AspNetCoreAssemblyCompiler(ICompilerLog compilerLog, ITemplateProvider templateProvider = null) : base(compilerLog, templateProvider)
        {
        }

        protected override List<SyntaxTree> CompileSource(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            string outputAuthoredSourceFolder)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            DirectoryInfo directoryInfo =  outputAuthoredSourceFolder != null ? new DirectoryInfo(outputAuthoredSourceFolder) : null;
            if (directoryInfo != null && !directoryInfo.Exists)
            {
                directoryInfo = null;
            }

            syntaxTrees.Add(CreateStartup(newAssemblyNamespace, directoryInfo));

            return syntaxTrees;
        }

        protected override List<ResourceDescription> CreateResources(string assemblyNamespace)
        {
            return null;
        }

        protected override IReadOnlyCollection<string> BuildCandidateReferenceList(CompileTargetEnum compileTarget, bool isFSharpProject)
        {
            throw new NotImplementedException();
        }

        private SyntaxTree CreateStartup(string namespaceName, DirectoryInfo directoryInfo)
        {
            string startupTemplateSource = TemplateProvider.GetTemplate("AspNetCore.startup","csharp");
            Func<object, string> template = Handlebars.Compile(startupTemplateSource);

            string outputCode = template(new
            {
                Namespace = namespaceName
            });
            OutputDiagnosticCode(directoryInfo, "Startup.cs", outputCode);
            
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode, path:$"Startup.cs");
            return syntaxTree;
        }
    }
}