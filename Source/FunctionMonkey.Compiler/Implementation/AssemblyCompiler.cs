using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Compiler.HandlebarsHelpers;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;
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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

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
            IReadOnlyCollection<string> externalAssemblyLocations,
            string outputBinaryFolder,
            string assemblyName,
            OpenApiOutputModel openApiOutputModel,
            FunctionCompiler.TargetEnum target,
            string outputAuthoredSourceFolder = null)
        {
            HandlebarsHelperRegistration.RegisterHelpers();
            IReadOnlyCollection<SyntaxTree> syntaxTrees = CompileSource(functionDefinitions,
                openApiOutputModel,
                functionAppConfigurationType,
                newAssemblyNamespace,
                outputAuthoredSourceFolder);

            CompileAssembly(syntaxTrees, externalAssemblyLocations, openApiOutputModel, outputBinaryFolder, assemblyName, newAssemblyNamespace, target);
        }

        private List<SyntaxTree> CompileSource(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            OpenApiOutputModel openApiOutputModel,
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
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, functionDefinition.Name, functionDefinition, directoryInfo, syntaxTrees);
            }

            if (openApiOutputModel != null && openApiOutputModel.IsConfiguredForUserInterface)
            {
                string templateSource = _templateProvider.GetTemplate("swaggerui","csharp");
                AddSyntaxTreeFromHandlebarsTemplate(templateSource, "SwaggerUi", new
                {
                    Namespace = newAssemblyNamespace
                }, directoryInfo, syntaxTrees);
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

        private static void AddSyntaxTreeFromHandlebarsTemplate(string templateSource, string name,
            object functionDefinition, DirectoryInfo directoryInfo, List<SyntaxTree> syntaxTrees)
        {
            Func<object, string> template = Handlebars.Compile(templateSource);

            string outputCode = template(functionDefinition);
            OutputDiagnosticCode(directoryInfo, name, outputCode);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode);
            syntaxTrees.Add(syntaxTree);
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
            IReadOnlyCollection<string> externalAssemblyLocations,
            OpenApiOutputModel openApiOutputModel,
            string outputBinaryFolder,
            string outputAssemblyName,
            string assemblyNamespace,
            FunctionCompiler.TargetEnum target)
        {
            IReadOnlyCollection<string> locations = BuildCandidateReferenceList(externalAssemblyLocations, target);
            const string manifestResourcePrefix = "FunctionMonkey.Compiler.references.netstandard2._0.";
            // For each assembly we've found we need to check and see if it is already included in the output binary folder
            // If it is then its referenced already by the function host and so we add a reference to that version.
            List<string> resolvedLocations = ResolveLocationsWithExistingReferences(outputBinaryFolder, locations);

            string[] manifestResoureNames = GetType().Assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(manifestResourcePrefix))
                .Select(x => x.Substring(manifestResourcePrefix.Length))
                .ToArray();

            List<PortableExecutableReference> references = BuildReferenceSet(resolvedLocations, manifestResoureNames, manifestResourcePrefix, target);
            CSharpCompilation compilation = CSharpCompilation.Create(outputAssemblyName,
                syntaxTrees,
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            List<ResourceDescription> resources = null;
            if (openApiOutputModel != null)
            {
                resources = new List<ResourceDescription>();
                Debug.Assert(openApiOutputModel.OpenApiSpecification != null);
                resources.Add(new ResourceDescription($"{assemblyNamespace}.OpenApi.{openApiOutputModel.OpenApiSpecification.Filename}",
                    () => new MemoryStream(Encoding.UTF8.GetBytes(openApiOutputModel.OpenApiSpecification.Content)), true));
                if (openApiOutputModel.SwaggerUserInterface != null)
                {
                    foreach (OpenApiFileReference fileReference in openApiOutputModel.SwaggerUserInterface)
                    {
                        OpenApiFileReference closureCapturedFileReference = fileReference;
                        resources.Add(new ResourceDescription($"{assemblyNamespace}.OpenApi.{closureCapturedFileReference.Filename}",
                            () => new MemoryStream(Encoding.UTF8.GetBytes(closureCapturedFileReference.Content)), true));
                    }
                }
            }
            
            using (Stream stream = new FileStream(Path.Combine(outputBinaryFolder, outputAssemblyName), FileMode.Create))
            {
                EmitResult result = compilation.Emit(stream, manifestResources: resources);
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

        private List<PortableExecutableReference> BuildReferenceSet(List<string> resolvedLocations,
            string[] manifestResoureNames,
            string manifestResourcePrefix,
            FunctionCompiler.TargetEnum target)
        {
            // Add our references - if the reference is to a library that forms part of NET Standard 2.0 then make sure we add
            // the reference from the embedded NET Standard reference set - although our target is NET Standard the assemblies
            // in the output folder of the Function App may be NET Core assemblies.
            List<PortableExecutableReference> references = resolvedLocations.Select(x =>
            {
                if (target == FunctionCompiler.TargetEnum.NETStandard20)
                {
                    string assemblyFilename = Path.GetFileName(x);
                    string manifestResourceName =
                        manifestResoureNames.SingleOrDefault(m =>
                            String.Equals(assemblyFilename, m, StringComparison.CurrentCultureIgnoreCase));
                    if (manifestResourceName != null)
                    {
                        using (Stream lib = GetType().Assembly
                            .GetManifestResourceStream(String.Concat(manifestResourcePrefix, manifestResourceName)))
                        {
                            return MetadataReference.CreateFromStream(lib);
                        }
                    }
                }

                return MetadataReference.CreateFromFile(x);

            }).ToList();

            if (target == FunctionCompiler.TargetEnum.NETStandard20)
            {
                using (Stream netStandard = GetType().Assembly
                    .GetManifestResourceStream("FunctionMonkey.Compiler.references.netstandard2._0.netstandard.dll"))
                {
                    references.Add(MetadataReference.CreateFromStream(netStandard));
                }

                using (Stream netStandard = GetType().Assembly
                    .GetManifestResourceStream("FunctionMonkey.Compiler.references.netstandard2._0.System.Runtime.dll"))
                {
                    references.Add(MetadataReference.CreateFromStream(netStandard));
                }

                using (Stream systemIo = GetType().Assembly
                    .GetManifestResourceStream(String.Concat(manifestResourcePrefix, "System.IO.dll")))
                {
                    references.Add(MetadataReference.CreateFromStream(systemIo));
                }
            }            
            
            return references;
        }

        private static IReadOnlyCollection<string> BuildCandidateReferenceList(IReadOnlyCollection<string> externalAssemblyLocations, FunctionCompiler.TargetEnum target)
        {
            // These are assemblies that Roslyn requires from usage within the template
            HashSet<string> locations = new HashSet<string>
            {
                typeof(Runtime).GetTypeInfo().Assembly.Location,
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
                typeof(TimerInfo).Assembly.Location,
                typeof(DbConnectionStringBuilder).Assembly.Location
            };

            if (target == FunctionCompiler.TargetEnum.NETCore21)
            {
                // we're a 2.1 assembly so we can use our assemblies
                Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                locations.Add(currentAssemblies.Single(x => x.GetName().Name == "netstandard").Location);
                locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Runtime").Location); // System.Runtime
                locations.Add(typeof(TargetFrameworkAttribute).Assembly.Location); // NetCoreLib
                locations.Add(typeof(System.Linq.Enumerable).Assembly.Location); // System.Linq
                locations.Add(typeof(System.Security.Claims.ClaimsPrincipal).Assembly.Location);
                locations.Add(typeof(System.Uri).Assembly.Location);
                locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Collections").Location);
                locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Threading").Location);
            }

            foreach (string externalAssemblyLocation in externalAssemblyLocations)
            {
                locations.Add(externalAssemblyLocation);
            }

            return locations;
        }

        private static List<string> ResolveLocationsWithExistingReferences(string outputBinaryFolder, IReadOnlyCollection<string> locations)
        {
            List<string> resolvedLocations = new List<string>(locations.Count);
            foreach (string location in locations)
            {
                // if the reference is already in the location
                if (Path.GetDirectoryName(location) == outputBinaryFolder)
                {
                    resolvedLocations.Add(location);
                    continue;
                }

                // if the assembly we've picked up from the compiler bundle is in the output folder then we use the one in
                // the output folder
                string pathInOutputFolder = Path.Combine(outputBinaryFolder, Path.GetFileName(location));
                if (File.Exists(pathInOutputFolder))
                {
                    resolvedLocations.Add(location);
                    continue;
                }

                resolvedLocations.Add(location);
            }

            return resolvedLocations;
        }
    }
}
