using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;
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
            string newAssemblyNamespace,
            DirectoryInfo outputAuthoredSourceFolder)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            
            HttpFunctionDefinition[] httpFunctions = functionDefinitions
                .Where(x => x is HttpFunctionDefinition)
                .Cast<HttpFunctionDefinition>()
                .ToArray();
            
            syntaxTrees.Add(CreateStartup(newAssemblyNamespace, outputAuthoredSourceFolder, httpFunctions));
            foreach (HttpFunctionDefinition httpFunctionDefinition in httpFunctions)
            {
                syntaxTrees.Add(CreateController(newAssemblyNamespace, outputAuthoredSourceFolder, httpFunctionDefinition));
            }

            return syntaxTrees;
        }
        
        protected override IReadOnlyCollection<string> BuildCandidateReferenceList(CompilerOptions compilerOptions, bool isFSharpProject)
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
                typeof(Task).Assembly.Location,
                typeof(Microsoft.AspNetCore.Authentication.AuthenticationBuilder).Assembly.Location,
                typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute).Assembly.Location,
                typeof(FunctionMonkey.AspNetCore.AuthenticationOptions).Assembly.Location,
                typeof(Microsoft.AspNetCore.Authentication.IAuthenticationHandler).Assembly.Location,
                typeof(Microsoft.AspNetCore.Builder.AuthorizationAppBuilderExtensions).Assembly.Location,
                typeof(Microsoft.Extensions.Logging.ILogger).Assembly.Location,
                typeof(Microsoft.Extensions.Configuration.IConfiguration).Assembly.Location,
                typeof(Microsoft.Extensions.Configuration.ConfigurationBinder).Assembly.Location,
                typeof(Microsoft.AspNetCore.Http.IHeaderDictionary).Assembly.Location,
                typeof(Microsoft.AspNetCore.Http.HttpRequest).Assembly.Location,
                typeof(Microsoft.Extensions.Primitives.StringValues).Assembly.Location,
                // Remove the below two lines when we can drop Newtonsoft Json - that requires resolving how to ignore
                // properties with 
                typeof(Microsoft.Extensions.DependencyInjection.NewtonsoftJsonMvcBuilderExtensions).Assembly.Location,
                typeof(Newtonsoft.Json.JsonSerializerSettings).Assembly.Location
            };

            return locations;
        }
        
        private SyntaxTree CreateStartup(string namespaceName,
            DirectoryInfo directoryInfo,
            IReadOnlyCollection<HttpFunctionDefinition> functions)
        {
            string startupTemplateSource = TemplateProvider.GetTemplate("startup","csharp");
            Func<object, string> template = Handlebars.Compile(startupTemplateSource);

            var startupOptions = new
            {
                Namespace = namespaceName,
                UsesTokenValidation = functions.Any(x => x.ValidatesToken),
                SetEnvironmentVariables =
                    true // this causes app settings specified like Azure Functions to be set as environment variables
            };
            string outputCode = template(startupOptions);
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