using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using FunctionMonkey.Compiler.HandlebarsHelpers;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;
using HandlebarsDotNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace FunctionMonkey.Compiler.Implementation
{
    internal class AssemblyCompiler : IAssemblyCompiler
    {
        private readonly ITemplateProvider _templateProvider;

        public AssemblyCompiler(ITemplateProvider templateProvider = null)
        {
            _templateProvider = templateProvider ?? new TemplateProvider();
        }

        public void Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type functionAppConfigurationType,
            string newAssemblyNamespace,
            IReadOnlyCollection<Assembly> externalAssemblies,
            string outputBinaryFolder,
            string assemblyName,
            bool openApiEndpointRequired,
            string outputAuthoredSourceFolder)
        {
            HandlebarsHelperRegistration.RegisterHelpers();
            IReadOnlyCollection<SyntaxTree> syntaxTrees = CompileSource(functionDefinitions,
                functionAppConfigurationType,
                newAssemblyNamespace,
                outputAuthoredSourceFolder);

            CompileAssembly(syntaxTrees, externalAssemblies, outputBinaryFolder, assemblyName);
        }

        private List<SyntaxTree> CompileSource(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type functionAppConfigurationType,
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
                string templateSource = _templateProvider.GetCSharpTemplate(functionDefinition);
                Func<object, string> template = Handlebars.Compile(templateSource);

                string outputCode = template(functionDefinition);
                OutputDiagnosticCode(directoryInfo, functionDefinition.Name, outputCode);
                
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode);
                syntaxTrees.Add(syntaxTree);
            }

            // Now we need to create a class that references the assembly with the configuration builder
            // otherwise the reference will be optimised away by Roslyn and it will then never get loaded
            // by the function host - and so at runtime the builder with the runtime info in won't be located
            string linkBackTemplateSource = _templateProvider.GetCSharpLinkBackTemplate();
            Func<object, string> linkBackTemplate = Handlebars.Compile(linkBackTemplateSource);
            LinkBackModel linkBackModel = new LinkBackModel
            {
                ConfigurationTypeName = functionAppConfigurationType.EvaluateType(),
                Namespace = newAssemblyNamespace
            };
            string outputLinkBackCode = linkBackTemplate(linkBackModel);
            OutputDiagnosticCode(directoryInfo, "ReferenceLinkBack", outputLinkBackCode);
            SyntaxTree linkBackSyntaxTree = CSharpSyntaxTree.ParseText(outputLinkBackCode);
            syntaxTrees.Add(linkBackSyntaxTree);

            return syntaxTrees;
        }

        private static void OutputDiagnosticCode(DirectoryInfo directoryInfo, string name,
            string outputCode)
        {
            if (directoryInfo != null)
            {
                using (StreamWriter writer =
                    File.CreateText(Path.Combine(directoryInfo.FullName, $"{name}.cs")))
                {
                    writer.Write(outputCode);
                }
            }
        }

        private void CompileAssembly(IReadOnlyCollection<SyntaxTree> syntaxTrees,
            IReadOnlyCollection<Assembly> externalAssemblies,
            string outputBinaryFolder,
            string outputAssemblyName)
        {
            // These are assemblies that Roslyn requires from usage within the template
            HashSet<string> locations = new HashSet<string>
            {
                typeof(Runtime).GetTypeInfo().Assembly.Location,
                typeof(AzureFromTheTrenches.Commanding.Abstractions.ICommand).GetTypeInfo().Assembly.Location,
                typeof(Abstractions.ICommandDeserializer).GetTypeInfo().Assembly.Location,
                typeof(System.Net.Http.HttpMethod).GetTypeInfo().Assembly.Location,
                typeof(System.Net.HttpStatusCode).GetTypeInfo().Assembly.Location,
                typeof(HttpRequest).Assembly.Location,
                typeof(object).GetTypeInfo().Assembly.Location,
                typeof(Hashtable).GetTypeInfo().Assembly.Location,
                typeof(JsonConvert).GetTypeInfo().Assembly.Location,
                typeof(OkObjectResult).GetTypeInfo().Assembly.Location,
                typeof(IActionResult).GetTypeInfo().Assembly.Location,
                typeof(FunctionNameAttribute).GetTypeInfo().Assembly.Location,
                typeof(ILogger).GetTypeInfo().Assembly.Location,
                typeof(IServiceProvider).GetTypeInfo().Assembly.Location,
                typeof(ClaimsPrincipal).GetTypeInfo().Assembly.Location,
                typeof(IHeaderDictionary).GetTypeInfo().Assembly.Location,
                typeof(StringValues).GetTypeInfo().Assembly.Location,
                Assembly.GetExecutingAssembly().Location,
                typeof(Attribute).Assembly.Location,
                Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll"),
                Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll"),
            };
            foreach (Assembly externalAssembly in externalAssemblies)
            {
                locations.Add(externalAssembly.Location);
            }

            PortableExecutableReference[] references = locations.Select(x => MetadataReference.CreateFromFile(x)).ToArray();
            var compilation = CSharpCompilation.Create(outputAssemblyName,
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (Stream stream = new FileStream(Path.Combine(outputBinaryFolder, outputAssemblyName), FileMode.Create))
            {
                EmitResult result = compilation.Emit(stream);
                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    StringBuilder messageBuilder = new StringBuilder();

                    foreach (Diagnostic diagnostic in failures)
                    {
                        messageBuilder.AppendFormat("{0}:{1} {2}", diagnostic.Id, diagnostic.GetMessage(), diagnostic.Location.ToString());
                    }

                    throw new ConfigurationException(messageBuilder.ToString());
                }
            }
        }
    }
}
