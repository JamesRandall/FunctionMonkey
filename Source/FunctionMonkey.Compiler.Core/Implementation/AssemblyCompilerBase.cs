using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Compiler.Core.Implementation.OpenApi;
using FunctionMonkey.Model;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.FSharp.Core;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal abstract class AssemblyCompilerBase : IAssemblyCompiler
    {
        protected AssemblyCompilerBase(ICompilerLog compilerLog, ITemplateProvider templateProvider)
        {
            CompilerLog = compilerLog;
            TemplateProvider = templateProvider;
        }

        public ICompilerLog CompilerLog { get; }

        public ITemplateProvider TemplateProvider { get; }
        
        public OpenApiOutputModel OpenApiOutputModel { get; set; }

        protected abstract List<SyntaxTree> CompileSource(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            string newAssemblyNamespace,
            DirectoryInfo outputAuthoredSourceFolder);

        protected List<ResourceDescription> CreateResources(string assemblyNamespace)
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

        protected abstract IReadOnlyCollection<string> BuildCandidateReferenceList(
            CompilerOptions compilerOptions,
            bool isFSharpProject);

        public bool Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            IReadOnlyCollection<string> externalAssemblyLocations,
            string outputBinaryFolder,
            string assemblyName,
            CompilerOptions compilerOptions,
            string outputAuthoredSourceFolder = null)
        {
            DirectoryInfo directoryInfo =  outputAuthoredSourceFolder != null ? new DirectoryInfo(outputAuthoredSourceFolder) : null;
            if (directoryInfo != null && !directoryInfo.Exists)
            {
                directoryInfo = null;
            }
            
            List<SyntaxTree> syntaxTrees = CompileSource(functionDefinitions,
                newAssemblyNamespace,
                directoryInfo).ToList();
            SyntaxTree linkBackTree = CreateLinkBack(functionDefinitions, backlinkType, backlinkPropertyInfo, newAssemblyNamespace, directoryInfo);
            if (linkBackTree != null)
            {
                syntaxTrees.Add(linkBackTree);
            }

            SyntaxTree openApiTree = CreateOpenApiTree(newAssemblyNamespace, directoryInfo);
            if (openApiTree != null)
            {
                syntaxTrees.Add(openApiTree);
            }

            bool isFSharpProject = functionDefinitions.Any(x => x.IsFunctionalFunction);

            return CompileAssembly(
                syntaxTrees,
                externalAssemblyLocations,
                outputBinaryFolder,
                assemblyName,
                newAssemblyNamespace,
                compilerOptions,
                isFSharpProject);
        }

        private SyntaxTree CreateOpenApiTree(string newAssemblyNamespace, DirectoryInfo directoryInfo)
        {
            if (OpenApiOutputModel != null && OpenApiOutputModel.IsConfiguredForUserInterface)
            {
                string templateSource = TemplateProvider.GetTemplate("swaggerui","csharp");
                return CreateSyntaxTreeFromHandlebarsTemplate(templateSource, "SwaggerUi", new
                {
                    Namespace = newAssemblyNamespace
                }, directoryInfo);
            }

            return null;
        }
        
        protected static SyntaxTree CreateSyntaxTreeFromHandlebarsTemplate(string templateSource, string name,
            object functionDefinition, DirectoryInfo directoryInfo)
        {
            Func<object, string> template = Handlebars.Compile(templateSource);

            string outputCode = template(functionDefinition);
            OutputDiagnosticCode(directoryInfo, name, outputCode);

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(outputCode, path:$"{name}.cs");
            return syntaxTree;
        }
        
        protected static List<string> ResolveLocationsWithExistingReferences(string outputBinaryFolder, IReadOnlyCollection<string> locations)
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
        
        protected List<PortableExecutableReference> BuildReferenceSet(List<string> resolvedLocations)
        {
            List<PortableExecutableReference> references =
                resolvedLocations.Select(x => MetadataReference.CreateFromFile(x)).ToList();
            return references;
        }
        
        protected SyntaxTree CreateLinkBack(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            DirectoryInfo outputAuthoredSourceFolder)
        {
            if (backlinkType == null) return null; // back link referencing has been disabled
            
            // Now we need to create a class that references the assembly with the configuration builder
            // otherwise the reference will be optimised away by Roslyn and it will then never get loaded
            // by the function host - and so at runtime the builder with the runtime info in won't be located
            string linkBackTemplateSource = TemplateProvider.GetCSharpLinkBackTemplate();
            Func<object, string> linkBackTemplate = Handlebars.Compile(linkBackTemplateSource);

            LinkBackModel linkBackModel = null;
            if (backlinkPropertyInfo != null)
            {
                linkBackModel = new LinkBackModel
                {
                    TypeName = backlinkType.EvaluateType(),
                    PropertyName = backlinkPropertyInfo.Name,
                    PropertyTypeName = backlinkPropertyInfo.PropertyType.EvaluateType(),
                    Namespace = newAssemblyNamespace
                };
            }
            else
            {
                linkBackModel = new LinkBackModel
                {
                    TypeName = backlinkType.EvaluateType(),
                    PropertyName = null,
                    Namespace = newAssemblyNamespace
                };
            }
            
            string outputLinkBackCode = linkBackTemplate(linkBackModel);
            OutputDiagnosticCode(outputAuthoredSourceFolder, "ReferenceLinkBack", outputLinkBackCode);
            SyntaxTree linkBackSyntaxTree = CSharpSyntaxTree.ParseText(outputLinkBackCode);
            return linkBackSyntaxTree;
        }

        private IReadOnlyCollection<string> GetReferenceLocations(
            IReadOnlyCollection<string> externalAssemblyLocations,
            CompilerOptions compilerOptions,
            bool isFSharpProject
            )
        {
            HashSet<string> locations =  new HashSet<string>(BuildCandidateReferenceList(compilerOptions, isFSharpProject));
            locations.Add(typeof(Task).GetTypeInfo().Assembly.Location);
            locations.Add(typeof(Runtime).GetTypeInfo().Assembly.Location);
            
            if (isFSharpProject)
            {
                locations.Add(typeof(FSharpOption<>).Assembly.Location);
            }

            
            Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            locations.Add(currentAssemblies.Single(x => x.GetName().Name == "netstandard").Location);
            locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Runtime").Location); // System.Runtime
            locations.Add(typeof(TargetFrameworkAttribute).Assembly.Location); // NetCoreLib
            locations.Add(typeof(System.Linq.Enumerable).Assembly.Location); // System.Linq
            locations.Add(typeof(System.Security.Claims.ClaimsPrincipal).Assembly.Location);
            locations.Add(typeof(System.Uri).Assembly.Location);
            locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Collections").Location);
            locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Threading").Location);
            locations.Add(currentAssemblies.Single(x => x.GetName().Name == "System.Threading.Tasks").Location);

                foreach (string externalAssemblyLocation in externalAssemblyLocations)
            {
                locations.Add(externalAssemblyLocation);
            }

            return locations;
        }
        
        private bool CompileAssembly(IReadOnlyCollection<SyntaxTree> syntaxTrees,
            IReadOnlyCollection<string> externalAssemblyLocations,
            string outputBinaryFolder,
            string outputAssemblyName,
            string assemblyNamespace,
            CompilerOptions compilerOptions,
            bool isFSharpProject)
        {
            IReadOnlyCollection<string> locations = GetReferenceLocations(externalAssemblyLocations, compilerOptions, isFSharpProject);
            const string manifestResourcePrefix = "FunctionMonkey.Compiler.references.netstandard2._0.";
            // For each assembly we've found we need to check and see if it is already included in the output binary folder
            // If it is then its referenced already by the function host and so we add a reference to that version.
            List<string> resolvedLocations = ResolveLocationsWithExistingReferences(outputBinaryFolder, locations);

            string[] manifestResoureNames = GetType().Assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(manifestResourcePrefix))
                .Select(x => x.Substring(manifestResourcePrefix.Length))
                .ToArray();

            List<PortableExecutableReference> references = BuildReferenceSet(resolvedLocations);
            
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyNamespace) //(outputAssemblyName)
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(references)
                    .AddSyntaxTrees(syntaxTrees)
                ;

            List<ResourceDescription> resources = CreateResources(assemblyNamespace);

            string outputFilename = Path.Combine(outputBinaryFolder, outputAssemblyName);
            EmitResult compilationResult = compilation.Emit(outputFilename, manifestResources: resources);
            if (!compilationResult.Success)
            {
                IEnumerable<Diagnostic> failures = compilationResult.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);
                    
                foreach (Diagnostic diagnostic in failures)
                {
                    CompilerLog.Error($"Error compiling function: {diagnostic.ToString()}");
                }
            }

            return compilationResult.Success;
        }
        
        protected static void OutputDiagnosticCode(DirectoryInfo directoryInfo, string name,
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
    }
}