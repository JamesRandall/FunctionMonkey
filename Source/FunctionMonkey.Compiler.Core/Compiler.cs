using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Builders;
using FunctionMonkey.Compiler.Core.Implementation;
using FunctionMonkey.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Compiler.Core
{
    public class Compiler
    {
        private readonly IServiceCollection _serviceCollection;

        private readonly Assembly _configurationSourceAssembly;
        private readonly ICompilerLog _compilerLog;
        private readonly ICommandRegistry _commandRegistry;
        private readonly ITriggerReferenceProvider _triggerReferenceProvider;
        private readonly string _outputBinaryFolder;
        
        public Compiler(Assembly configurationSourceAssembly,
            string outputBinaryFolder,
            ICompilerLog compilerLog)
        {
            _configurationSourceAssembly = configurationSourceAssembly;
            _outputBinaryFolder = outputBinaryFolder;
            _compilerLog = compilerLog;
            _serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => _serviceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => _serviceCollection.AddTransient(fromType, toType),
                (resolveType) => null // we never resolve during compilation
            );
            _commandRegistry = adapter.AddCommanding();
            _triggerReferenceProvider = new TriggerReferenceProvider();
        }

        public bool Compile()
        {
            string newAssemblyNamespace = $"{_configurationSourceAssembly.GetName().Name.Replace("-", "_")}.Functions";
            IFunctionCompilerMetadata functionCompilerMetadata = null;
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration(_configurationSourceAssembly);
            if (configuration == null)
            {
                functionCompilerMetadata = ConfigurationLocator.FindCompilerMetadata(_configurationSourceAssembly);
                if (functionCompilerMetadata == null)
                {
                    _compilerLog.Error($"The assembly {_configurationSourceAssembly.GetName().Name} does not contain a public class implementing the IFunctionAppConfiguration interface");
                    return false;
                }
            }
            else
            {
                FunctionHostBuilder builder = new FunctionHostBuilder(_serviceCollection, _commandRegistry, false);
                configuration.Build(builder);
                new PostBuildPatcher().Patch(builder, newAssemblyNamespace);
                if (!VerifyCommandAndResponseTypes(builder))
                {
                    return false;
                }

                functionCompilerMetadata = new FunctionCompilerMetadata
                {
                    FunctionDefinitions = builder.FunctionDefinitions,
                    OpenApiConfiguration = builder.OpenApiConfiguration,
                    OutputAuthoredSourceFolder = builder.OutputAuthoredSourceFolder,
                    CompileTarget = builder.Options.HttpTarget
                };
            }
            
            IReadOnlyCollection<string> externalAssemblies =
                GetExternalAssemblyLocations(functionCompilerMetadata.FunctionDefinitions);

            ITargetCompiler targetCompiler = functionCompilerMetadata.CompileTarget == CompileTargetEnum.AzureFunctions
                ? (ITargetCompiler)new AzureFunctionsCompiler(_compilerLog)
                : new AspNetCoreCompiler(_compilerLog); 

            return targetCompiler.CompileAssets(functionCompilerMetadata,
                newAssemblyNamespace,
                configuration,
                externalAssemblies,
                _outputBinaryFolder);
        }

        private bool VerifyCommandAndResponseTypes(FunctionHostBuilder builder)
        {
            bool hasErrors = false;
            foreach (AbstractFunctionDefinition functionDefinition in builder.FunctionDefinitions)
            {
                if (!functionDefinition.CommandType.IsPublic)
                {
                    _compilerLog.Error($"Command {functionDefinition.CommandType.FullName} must be public");
                    hasErrors = true;
                }

                if (functionDefinition.CommandResultType != null && !functionDefinition.CommandResultType.IsPublic)
                {
                    _compilerLog.Error($"Command result type {functionDefinition.CommandResultType.FullName} must be public");
                    hasErrors = true;
                }
            }

            return !hasErrors;
        }        

        private IReadOnlyCollection<string> GetExternalAssemblyLocations(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            HashSet<Assembly> assemblies = new HashSet<Assembly>();

            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                assemblies.Add(_triggerReferenceProvider.GetTriggerReference(functionDefinition));
                assemblies.Add(functionDefinition.CommandType.Assembly);
                foreach (Type commandInterface in functionDefinition.CommandType.GetInterfaces())
                {
                    assemblies.Add(commandInterface.Assembly);
                }

                if (functionDefinition.CommandResultType != null)
                {
                    // skip system types
                    if (functionDefinition.CommandResultType.Assembly != typeof(string).Assembly)
                    {
                        assemblies.Add(functionDefinition.CommandResultType.Assembly);
                    }
                }
            }

            // TODO: Do we need this any more? We no longer run the startup code in the compilation process?
            foreach (ServiceDescriptor descriptor in _serviceCollection)
            {
                assemblies.Add(descriptor.ServiceType.Assembly);
                if (descriptor.ImplementationType != null)
                {
                    assemblies.Add(descriptor.ImplementationType.Assembly);
                }

                if (descriptor.ImplementationInstance != null)
                {
                    assemblies.Add(descriptor.ImplementationInstance.GetType().Assembly);
                }
            }

            IRegistrationCatalogue catalogue = (IRegistrationCatalogue)_commandRegistry;

            foreach (Type handler in catalogue.GetRegisteredHandlers())
            {
                assemblies.Add(handler.Assembly);
            }

            foreach (Type command in catalogue.GetRegisteredCommands())
            {
                assemblies.Add(command.Assembly);
            }

            assemblies.Add(_configurationSourceAssembly);

            // we have to add directly referenced assemblies in case the commands and result types make use of external types
            // TODO: their is an argument to restricting this
            foreach (Assembly assembly in assemblies.ToArray())
            {
                AssemblyName[] referencedAssemblies = assembly.GetReferencedAssemblies();
                foreach (var referencedAssemblyName in referencedAssemblies)
                {
                    if (referencedAssemblyName.Name == "netstandard" || referencedAssemblyName.Name == "System.Runtime")
                    {
                        continue;
                    }
                    var referencedAssembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.FullName == referencedAssemblyName.FullName);
                    if (referencedAssembly != null)
                    {
                        assemblies.Add(referencedAssembly);
                    }
                }
            }

            // at the moment we can't get the actual dispatcher types without actually calling the function and looking at ther result - needs thought
            return assemblies.Select(x => x.Location).ToArray();
        }
    }
}