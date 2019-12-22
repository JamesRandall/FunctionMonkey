using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FunctionMonkey.Compiler.Core.Implementation.AspNetCore
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
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in functionDefinitions)
            {
                if (abstractFunctionDefinition is HttpFunctionDefinition httpFunctionDefinition)
                {
                    syntaxTrees.Add(CreateController(newAssemblyNamespace, directoryInfo, httpFunctionDefinition));
                }
            }

            return syntaxTrees;
        }

        protected override List<ResourceDescription> CreateResources(string assemblyNamespace)
        {
            return null;
        }

        protected override IReadOnlyCollection<string> BuildCandidateReferenceList(CompileTargetEnum compileTarget, bool isFSharpProject)
        {
            HashSet<string> locations = new HashSet<string>
            {
                typeof(IApplicationBuilder).Assembly.Location,
                typeof(IWebHostBuilder).Assembly.Location,
                typeof(IWebHostEnvironment).Assembly.Location,
                typeof(IServiceCollection).Assembly.Location,
                typeof(IServiceProvider).Assembly.Location,
                typeof(IHostEnvironment).Assembly.Location,
                typeof(DeveloperExceptionPageExtensions).Assembly.Location,
                typeof(HttpContext).Assembly.Location,
                typeof(EndpointRoutingApplicationBuilderExtensions).Assembly.Location,
                typeof(MvcServiceCollectionExtensions).Assembly.Location,
                typeof(IMvcBuilder).Assembly.Location,
                typeof(IEndpointRouteBuilder).Assembly.Location,
                typeof(ICommandDispatcher).Assembly.Location,
                typeof(ActionResult).Assembly.Location,
                typeof(IActionResult).Assembly.Location,
                typeof(Task).Assembly.Location
            };

            return locations;
        }
        
        private SyntaxTree CreateStartup(string namespaceName, DirectoryInfo directoryInfo)
        {
            string startupTemplateSource = TemplateProvider.GetTemplate("startup","csharp");
            Func<object, string> template = Handlebars.Compile(startupTemplateSource);

            string outputCode = template(new
            {
                Namespace = namespaceName,
                OpenApiEnabled = false
            });
            OutputDiagnosticCode(directoryInfo, "Startup", outputCode);
            
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode, path:$"Startup.cs");
            return syntaxTree;
        }
        
        private SyntaxTree CreateController(string namespaceName, DirectoryInfo directoryInfo, HttpFunctionDefinition httpFunctionDefinition)
        {
            string startupTemplateSource = TemplateProvider.GetTemplate("controller","csharp");
            Func<object, string> template = Handlebars.Compile(startupTemplateSource);
            string filenameWithoutExtension = $"{httpFunctionDefinition.Name}Controller";

            string outputCode = template(httpFunctionDefinition);
            OutputDiagnosticCode(directoryInfo, filenameWithoutExtension, outputCode);
            
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode, path:$"{filenameWithoutExtension}.cs");
            return syntaxTree;
        }
    }
}