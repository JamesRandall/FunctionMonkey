using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Extensions;
using FunctionMonkey.Compiler.Core.HandlebarsHelpers;
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

        protected abstract List<SyntaxTree> CompileSource(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            OpenApiOutputModel openApiOutputModel,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            string outputAuthoredSourceFolder);

        protected abstract List<ResourceDescription> CreateResources(OpenApiOutputModel openApiOutputModel,
            string assemblyNamespace);

        protected abstract IReadOnlyCollection<string> BuildCandidateReferenceList(
            CompileTargetEnum compileTarget,
            bool isFSharpProject);
        
        public bool Compile(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            IReadOnlyCollection<string> externalAssemblyLocations,
            string outputBinaryFolder,
            string assemblyName,
            OpenApiOutputModel openApiOutputModel,
            CompileTargetEnum compileTarget,
            string outputAuthoredSourceFolder = null)
        {
            HandlebarsHelperRegistration.RegisterHelpers();
            IReadOnlyCollection<SyntaxTree> syntaxTrees = CompileSource(functionDefinitions,
                openApiOutputModel,
                backlinkType,
                backlinkPropertyInfo,
                newAssemblyNamespace,
                outputAuthoredSourceFolder);

            bool isFSharpProject = functionDefinitions.Any(x => x.IsFunctionalFunction);

            return CompileAssembly(
                syntaxTrees,
                externalAssemblyLocations,
                openApiOutputModel,
                outputBinaryFolder,
                assemblyName,
                newAssemblyNamespace,
                compileTarget,
                isFSharpProject);
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
        
        protected List<PortableExecutableReference> BuildReferenceSet(List<string> resolvedLocations,
            string[] manifestResoureNames,
            string manifestResourcePrefix,
            CompileTargetEnum compileTarget)
        {
            List<PortableExecutableReference> references =
                resolvedLocations.Select(x => MetadataReference.CreateFromFile(x)).ToList();
            // Add our references - if the reference is to a library that forms part of NET Standard 2.0 then make sure we add
            // the reference from the embedded NET Standard reference set - although our target is NET Standard the assemblies
            // in the output folder of the Function App may be NET Core assemblies.
            /*List<PortableExecutableReference> references = resolvedLocations.Select(x =>
            {
                if (compileTarget == CompileTargetEnum.NETStandard20)
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

            }).ToList();*/

            /*if (compileTarget == CompileTargetEnum.NETStandard20)
            {
                using (Stream netStandard = GetType().Assembly
                    .GetManifestResourceStream("FunctionMonkey.Compiler.Core.references.netstandard2._0.netstandard.dll"))
                {
                    references.Add(MetadataReference.CreateFromStream(netStandard));
                }

                using (Stream netStandard = GetType().Assembly
                    .GetManifestResourceStream("FunctionMonkey.Compiler.Core.references.netstandard2._0.System.Runtime.dll"))
                {
                    references.Add(MetadataReference.CreateFromStream(netStandard));
                }

                using (Stream systemIo = GetType().Assembly
                    .GetManifestResourceStream(String.Concat(manifestResourcePrefix, "System.IO.dll")))
                {
                    references.Add(MetadataReference.CreateFromStream(systemIo));
                }
            }*/

            return references;
        }
        
        protected void CreateLinkBack(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions,
            Type backlinkType,
            PropertyInfo backlinkPropertyInfo,
            string newAssemblyNamespace,
            DirectoryInfo directoryInfo,
            List<SyntaxTree> syntaxTrees)
        {
            if (backlinkType == null) return; // back link referencing has been disabled
            
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
            OutputDiagnosticCode(directoryInfo, "ReferenceLinkBack", outputLinkBackCode);
            SyntaxTree linkBackSyntaxTree = CSharpSyntaxTree.ParseText(outputLinkBackCode);
            syntaxTrees.Add(linkBackSyntaxTree);
        }

        private IReadOnlyCollection<string> GetReferenceLocations(
            IReadOnlyCollection<string> externalAssemblyLocations,
            CompileTargetEnum compileTarget,
            bool isFSharpProject
            )
        {
            HashSet<string> locations =  new HashSet<string>(BuildCandidateReferenceList(compileTarget, isFSharpProject));
            locations.Add(typeof(Task).GetTypeInfo().Assembly.Location);
            locations.Add(typeof(Runtime).GetTypeInfo().Assembly.Location);
            
            if (isFSharpProject)
            {
                locations.Add(typeof(FSharpOption<>).Assembly.Location);
            }

            //if (compileTarget == CompileTargetEnum.NETCore21)
            //{
                // we're a 3.x assembly so we can use our assemblies
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
            //}
            
            foreach (string externalAssemblyLocation in externalAssemblyLocations)
            {
                locations.Add(externalAssemblyLocation);
            }

            return locations;
        }
        
        private bool CompileAssembly(IReadOnlyCollection<SyntaxTree> syntaxTrees,
            IReadOnlyCollection<string> externalAssemblyLocations,
            OpenApiOutputModel openApiOutputModel,
            string outputBinaryFolder,
            string outputAssemblyName,
            string assemblyNamespace,
            CompileTargetEnum compileTarget,
            bool isFSharpProject)
        {
            IReadOnlyCollection<string> locations = GetReferenceLocations(externalAssemblyLocations, compileTarget, isFSharpProject);
            const string manifestResourcePrefix = "FunctionMonkey.Compiler.references.netstandard2._0.";
            // For each assembly we've found we need to check and see if it is already included in the output binary folder
            // If it is then its referenced already by the function host and so we add a reference to that version.
            List<string> resolvedLocations = ResolveLocationsWithExistingReferences(outputBinaryFolder, locations);

            string[] manifestResoureNames = GetType().Assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith(manifestResourcePrefix))
                .Select(x => x.Substring(manifestResourcePrefix.Length))
                .ToArray();

            List<PortableExecutableReference> references = BuildReferenceSet(resolvedLocations, manifestResoureNames, manifestResourcePrefix, compileTarget);
            
            CSharpCompilation compilation = CSharpCompilation.Create(assemblyNamespace) //(outputAssemblyName)
                    .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                    .AddReferences(references)
                    .AddSyntaxTrees(syntaxTrees)
                ;

            List<ResourceDescription> resources = CreateResources(openApiOutputModel, assemblyNamespace);

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