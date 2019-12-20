using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Compiler.Core.HandlebarsHelpers;
using FunctionMonkey.SignalR;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.FSharp.Core;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal class AzureFunctionsAssemblyCompiler : AssemblyCompilerBase
    {
        public AzureFunctionsAssemblyCompiler(ICompilerLog compilerLog, ITemplateProvider templateProvider) : base(compilerLog, templateProvider)
        {
            
        }
        
        public OpenApiOutputModel OpenApiOutputModel { get; set; }

        protected override List<SyntaxTree> CompileSource(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
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
            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                string templateSource = TemplateProvider.GetCSharpTemplate(functionDefinition);
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, functionDefinition.Name, functionDefinition, directoryInfo, syntaxTrees);
            }

            if (OpenApiOutputModel != null && OpenApiOutputModel.IsConfiguredForUserInterface)
            {
                string templateSource = TemplateProvider.GetTemplate("swaggerui","csharp");
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, "SwaggerUi", new
                {
                    Namespace = newAssemblyNamespace
                }, directoryInfo, syntaxTrees);
            }

            {
                string templateSource = TemplateProvider.GetTemplate("startup", "csharp");
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, "Startup", new
                {
                    Namespace = newAssemblyNamespace
                }, directoryInfo, syntaxTrees);
            }

            CreateLinkBack(functionDefinitions, backlinkType, backlinkPropertyInfo, newAssemblyNamespace, directoryInfo, syntaxTrees);

            return syntaxTrees;
        }
        

        private static void AddSyntaxTreeFromHandlebarsTemplate(string templateSource, string name,
            object functionDefinition, DirectoryInfo directoryInfo, List<SyntaxTree> syntaxTrees)
        {
            Func<object, string> template = Handlebars.Compile(templateSource);

            string outputCode = template(functionDefinition);
            OutputDiagnosticCode(directoryInfo, name, outputCode);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode, path:$"{name}.cs");
            //syntaxTree = syntaxTree.WithFilePath($"{name}.cs");
            syntaxTrees.Add(syntaxTree);
        }
        
        protected override List<ResourceDescription> CreateResources(string assemblyNamespace)
        {
            List<ResourceDescription> resources = null;
            if (OpenApiOutputModel != null)
            {
                resources = new List<ResourceDescription>();
                Debug.Assert(OpenApiOutputModel.OpenApiSpecification != null);
                resources.Add(new ResourceDescription(
                    $"{assemblyNamespace}.OpenApi.{OpenApiOutputModel.OpenApiSpecification.Filename}",
                    () => new MemoryStream(Encoding.UTF8.GetBytes(OpenApiOutputModel.OpenApiSpecification.Content)), true));
                if (OpenApiOutputModel.SwaggerUserInterface != null)
                {
                    foreach (OpenApiFileReference fileReference in OpenApiOutputModel.SwaggerUserInterface)
                    {
                        OpenApiFileReference closureCapturedFileReference = fileReference;
                        resources.Add(new ResourceDescription(
                            $"{assemblyNamespace}.OpenApi.{closureCapturedFileReference.Filename}",
                            () => new MemoryStream(Encoding.UTF8.GetBytes(closureCapturedFileReference.Content)), true));
                    }
                }
            }

            return resources;
        }

        protected override IReadOnlyCollection<string> BuildCandidateReferenceList(
            CompileTargetEnum compileTarget,
            bool isFSharpProject)
        {
            // These are assemblies that Roslyn requires from usage within the template
            HashSet<string> locations = new HashSet<string>
            {
                typeof(IStreamCommand).Assembly.Location,
                typeof(AzureFromTheTrenches.Commanding.Abstractions.ICommand).GetTypeInfo().Assembly.Location,
                typeof(Abstractions.ISerializer).GetTypeInfo().Assembly.Location,
                typeof(System.Net.Http.HttpMethod).GetTypeInfo().Assembly.Location,
                typeof(System.Net.HttpStatusCode).GetTypeInfo().Assembly.Location,
                typeof(HttpRequest).Assembly.Location,
                typeof(JsonConvert).GetTypeInfo().Assembly.Location,
                typeof(OkObjectResult).GetTypeInfo().Assembly.Location,
                typeof(IActionResult).GetTypeInfo().Assembly.Location,
                typeof(FunctionNameAttribute).GetTypeInfo().Assembly.Location,
                typeof(ILogger).GetTypeInfo().Assembly.Location,
                typeof(IServiceProvider).GetTypeInfo().Assembly.Location,
                typeof(IHeaderDictionary).GetTypeInfo().Assembly.Location,
                typeof(StringValues).GetTypeInfo().Assembly.Location,
                typeof(ExecutionContext).GetTypeInfo().Assembly.Location,
                typeof(Document).GetTypeInfo().Assembly.Location,
                typeof(Message).GetTypeInfo().Assembly.Location,
                typeof(ChangeFeedProcessorBuilder).Assembly.Location,
                typeof(CosmosDBAttribute).Assembly.Location,
                typeof(TimerInfo).Assembly.Location,
                typeof(DbConnectionStringBuilder).Assembly.Location,
                typeof(AzureSignalRAuthClient).Assembly.Location,
                typeof(System.Environment).Assembly.Location,
                typeof(HttpTriggerAttribute).Assembly.Location,
                typeof(ServiceBusAttribute).Assembly.Location,
                typeof(QueueAttribute).Assembly.Location,
                typeof(Microsoft.IdentityModel.Protocols.HttpDocumentRetriever).Assembly.Location,
                typeof(IServiceCollection).Assembly.Location
            };
            return locations;
        }
    }
}
