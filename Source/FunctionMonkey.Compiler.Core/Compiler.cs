using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.SignalR;
using FunctionMonkey.Builders;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Compiler.Core.Implementation;
using FunctionMonkey.Compiler.Core.Implementation.AspNetCore;
using FunctionMonkey.Compiler.Core.Implementation.AzureFunctions;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using FunctionMonkey.Model.OutputBindings;
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

            IFunctionAppConfiguration configuration = null;
            FunctionAppHostBuilder appHostBuilder = null;
            IFunctionAppHost appHost = ConfigurationLocator.FindFunctionAppHost(_configurationSourceAssembly);
            
            if (appHost != null)
            {
                appHostBuilder = new FunctionAppHostBuilder();
                appHost.Build(appHostBuilder);
                if (appHostBuilder.FunctionAppConfiguration != null)
                {
                    configuration = (IFunctionAppConfiguration)Activator.CreateInstance(appHostBuilder.FunctionAppConfiguration);
                }
            }

            if (configuration == null)
            {
                configuration = ConfigurationLocator.FindConfiguration(_configurationSourceAssembly);
            }

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
                if (appHostBuilder != null)
                {
                    builder.Options = appHostBuilder.Options;
                }
                configuration.Build(builder);
                DefaultMediatorSettings.SetDefaultsIfRequired(builder);
                if (!ValidateCommandTypes(builder))
                {
                    return false;
                }
                IMediatorResultTypeExtractor extractor = CreateMediatorResultTypeExtractor(builder.Options.MediatorResultTypeExtractor);
                if (extractor == null)
                {
                    return false;
                }
                new PostBuildPatcher(extractor).Patch(builder, newAssemblyNamespace);
                if (!VerifyCommandAndResponseTypes(builder))
                {
                    return false;
                }

                if (!VerifyOutputBindings(builder))
                {
                    return false;
                }

                functionCompilerMetadata = new FunctionCompilerMetadata
                {
                    FunctionDefinitions = builder.FunctionDefinitions,
                    OpenApiConfiguration = builder.OpenApiConfiguration,
                    OutputAuthoredSourceFolder = builder.Options.OutputSourceTo,
                    CompilerOptions = builder.Options
                };
            }

            PostBuildPatcher.EnsureFunctionsHaveUniqueNames(functionCompilerMetadata.FunctionDefinitions);
            
            IReadOnlyCollection<string> externalAssemblies =
                GetExternalAssemblyLocations(functionCompilerMetadata.FunctionDefinitions);

            ITargetCompiler targetCompiler = functionCompilerMetadata.CompilerOptions.HttpTarget == CompileTargetEnum.AzureFunctions
                ? (ITargetCompiler)new AzureFunctionsCompiler(_compilerLog)
                : new AspNetCoreCompiler(_compilerLog); 

            return targetCompiler.CompileAssets(functionCompilerMetadata,
                newAssemblyNamespace,
                configuration,
                externalAssemblies,
                _outputBinaryFolder);
        }

        private bool VerifyOutputBindings(IFunctionHostBuilder builder)
        {
            bool foundErrors = false;
            foreach (AbstractFunctionDefinition functionDefinition in builder.FunctionDefinitions)
            {
                if (functionDefinition.OutputBinding != null)
                {
                    if (!functionDefinition.CommandHasResult &&
                        !(functionDefinition.NoCommandHandler || functionDefinition.CommandType.GetInterfaces().Any(x => x == typeof(ICommandWithNoHandler))))
                    {
                        _compilerLog.Error($"Command of type {functionDefinition.CommandType.Name} requires a result to be used with an output binding");
                        foundErrors = true;
                    }

                    if (functionDefinition.OutputBinding is SignalROutputBinding signalROutputBinding)
                    {
                        if (signalROutputBinding.SignalROutputTypeName == SignalROutputBinding.SignalROutputMessageType)
                        {
                            if (signalROutputBinding.OutputBindingConverterType == null && !typeof(SignalRMessage).IsAssignableFrom(functionDefinition.CommandResultItemType))
                            {
                                _compilerLog.Error("Commands that use SignalRMessage output bindings must return a FunctionMonkey.Abstractions.SignalR.SignalRMessage class, a derivative, or use an output converter");
                                foundErrors = true;
                            }
                        }
                        else if (signalROutputBinding.SignalROutputTypeName == SignalROutputBinding.SignalROutputGroupActionType)
                        {
                            if (signalROutputBinding.OutputBindingConverterType == null && !typeof(SignalRGroupAction).IsAssignableFrom(functionDefinition.CommandResultItemType))
                            {
                                _compilerLog.Error("Commands that use SignalRGroupAction output bindings must return a FunctionMonkey.Abstractions.SignalR.SignalRGroupAction class, a derivative, or use an output converter");
                                foundErrors = true;
                            }
                        }
                        
                    }
                }
            }
            

            return !foundErrors;
        }

        private IMediatorResultTypeExtractor CreateMediatorResultTypeExtractor(Type optionsMediatorResultTypeExtractor)
        {
            ConstructorInfo constructor = optionsMediatorResultTypeExtractor.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                _compilerLog.Error($"{optionsMediatorResultTypeExtractor.Name} does not have a default constructor. Implementations of IMediatorResultTypeExtractor must have a default (parameterless) constructor.");
                return null;
            }

            return (IMediatorResultTypeExtractor)Activator.CreateInstance(optionsMediatorResultTypeExtractor);
        }

        private bool ValidateCommandTypes(FunctionHostBuilder builder)
        {
            // TODO: Add a check in the assembvly compile type checker to make sure that Negotiate<TCommand> type commands have a return type of SignalRNegotiateResponse
            
            
            ConstructorInfo constructor = builder.Options.MediatorTypeSafetyEnforcer.GetConstructor(new Type[0]);
            if (constructor == null)
            {
                _compilerLog.Error($"{builder.Options.MediatorTypeSafetyEnforcer.Name} does not have a default constructor. Implementations of IMediatorTypeSafetyEnforcer must have a default (parameterless) constructor.");
                return false;
            }

            IMediatorTypeSafetyEnforcer typeSafetyEnforcer = (IMediatorTypeSafetyEnforcer)Activator.CreateInstance(builder.Options.MediatorTypeSafetyEnforcer);
            bool errorFound = false;
            foreach (AbstractFunctionDefinition functionDefinition in builder.FunctionDefinitions)
            {
                // SignalRBindingExpressionNegotiateCommand is a type used only to make a non dispatching HTTP function
                // definition work, it doesn't get used with any mediator and is defined within Function Monkey. It is
                // exempt from mediator type checking
                if (functionDefinition.CommandType != typeof(SignalRBindingExpressionNegotiateCommand) &&
                    functionDefinition.CommandType != typeof(SignalRClaimTypeNegotiateCommand))
                {
                    if (!typeSafetyEnforcer.IsValidType(functionDefinition.CommandType))
                    {
                        errorFound = true;
                        _compilerLog.Error($"Command type {functionDefinition.CommandType.Name} does not conform to the requirements of the mediator. {typeSafetyEnforcer.Requirements}");
                    }
                }
            }

            return !errorFound;
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