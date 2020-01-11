using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Compiler.Core.HandlebarsHelpers.AzureFunctions;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;
using FunctionMonkey.Model;
using FunctionMonkey.SignalR;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace FunctionMonkey.Compiler.Core.Implementation.AzureFunctions
{
    internal class AzureFunctionsAssemblyCompiler : AssemblyCompilerBase
    {
        public AzureFunctionsAssemblyCompiler(ICompilerLog compilerLog, ITemplateProvider templateProvider) : base(compilerLog, templateProvider)
        {
            
        }
        
        protected override List<SyntaxTree> CompileSource(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            string newAssemblyNamespace,
            DirectoryInfo outputAuthoredSourceFolder)
        {
            List<SyntaxTree> syntaxTrees = new List<SyntaxTree>();
            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                string templateSource = TemplateProvider.GetCSharpTemplate(functionDefinition);
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, functionDefinition.Name, functionDefinition, outputAuthoredSourceFolder, syntaxTrees);
            }
            
            {
                string templateSource = TemplateProvider.GetTemplate("startup", "csharp");
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, "Startup", new
                {
                    Namespace = newAssemblyNamespace
                }, outputAuthoredSourceFolder, syntaxTrees);
            }

            return syntaxTrees;
        }
        

        private static void AddSyntaxTreeFromHandlebarsTemplate(string templateSource, string name,
            object functionDefinition, DirectoryInfo directoryInfo, List<SyntaxTree> syntaxTrees)
        {
            SyntaxTree syntaxTree =
                CreateSyntaxTreeFromHandlebarsTemplate(templateSource, name, functionDefinition, directoryInfo);
            syntaxTrees.Add(syntaxTree);
        }
        
        protected override IReadOnlyCollection<string> BuildCandidateReferenceList(
            CompilerOptions compilerOptions,
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
                typeof(IServiceCollection).Assembly.Location,
                typeof(EventData).Assembly.Location
            };
            return locations;
        }
    }
}
