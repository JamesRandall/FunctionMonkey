using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Builders;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Compiler.Implementation
{
    internal class FunctionCompiler
    {
        private readonly IServiceCollection _serviceCollection;

        private readonly Assembly _configurationSourceAssembly;
        private readonly string _outputBinaryFolder;
        private readonly bool _outputProxiesJson;
        private readonly ICommandRegistry _commandRegistry;
        private readonly IAssemblyCompiler _assemblyCompiler;
        private readonly ITriggerReferenceProvider _triggerReferenceProvider;
        private readonly JsonCompiler _jsonCompiler;
        private readonly ProxiesJsonCompiler _proxiesJsonCompiler;
        private readonly OpenApiCompiler _openApiCompiler;

        public FunctionCompiler(Assembly configurationSourceAssembly,
            string outputBinaryFolder,
            bool outputProxiesJson,
            IAssemblyCompiler assemblyCompiler = null,
            ITriggerReferenceProvider triggerReferenceProvider = null)
        {
            _configurationSourceAssembly = configurationSourceAssembly;
            _outputBinaryFolder = outputBinaryFolder;
            _outputProxiesJson = outputProxiesJson;
            _serviceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => _serviceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => _serviceCollection.AddTransient(fromType, toType),
                (resolveType) => null // we never resolve during compilation
            );
            _commandRegistry = adapter.AddCommanding();
            _assemblyCompiler = assemblyCompiler ?? new AssemblyCompiler();
            _triggerReferenceProvider = triggerReferenceProvider ?? new TriggerReferenceProvider();
            _jsonCompiler = new JsonCompiler();
            _proxiesJsonCompiler = new ProxiesJsonCompiler();
            _openApiCompiler = new OpenApiCompiler();
        }

        public void Compile()
        {
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration(_configurationSourceAssembly);
            if (configuration == null)
            {
                throw new ConfigurationException($"The assembly {_configurationSourceAssembly.GetName().Name} does not contain a public class implementing the IFunctionAppConfiguration interface");
            }

            string newAssemblyNamespace = $"{_configurationSourceAssembly.GetName().Name}.Functions";
            FunctionHostBuilder builder = new FunctionHostBuilder(_serviceCollection, _commandRegistry, false);
            configuration.Build(builder);
            new PostBuildPatcher().Patch(builder, newAssemblyNamespace);
            
            IReadOnlyCollection<Assembly> externalAssemblies = GetExternalAssemblies(builder.FunctionDefinitions);
            _jsonCompiler.Compile(builder.FunctionDefinitions, _outputBinaryFolder, newAssemblyNamespace);
            if (_outputProxiesJson && builder.AreProxiesEnabled)
            {
                _proxiesJsonCompiler.Compile(builder.FunctionDefinitions, _outputBinaryFolder);
            }
            bool openApiEndpointRequired = _openApiCompiler.Compile(builder.OpenApiConfiguration, builder.FunctionDefinitions, _outputBinaryFolder);

            _assemblyCompiler.Compile(builder.FunctionDefinitions,
                configuration.GetType(),
                newAssemblyNamespace,
                externalAssemblies, 
                _outputBinaryFolder, 
                $"{newAssemblyNamespace}.dll", 
                openApiEndpointRequired, 
                builder.OutputAuthoredSourceFolder);
        }

        private IReadOnlyCollection<Assembly> GetExternalAssemblies(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            HashSet<Assembly> assemblies = new HashSet<Assembly>();

            foreach (AbstractFunctionDefinition functionDefinition in functionDefinitions)
            {
                assemblies.Add(_triggerReferenceProvider.GetTriggerReference(functionDefinition));
                assemblies.Add(functionDefinition.CommandType.Assembly);                

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
            return assemblies;
        }
    }
}
