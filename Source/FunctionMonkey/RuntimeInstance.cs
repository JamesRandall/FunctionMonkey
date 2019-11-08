using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Builders;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FunctionMonkey
{
    public class RuntimeInstance
    {
        public IServiceProvider ServiceProvider { get; }

        private IServiceCollection ServiceCollection { get; }

        public AsyncLocal<ILogger> FunctionProvidedLogger { get;  }= new AsyncLocal<ILogger>(null);

        public IFunctionHostBuilder Builder { get; }

        public RuntimeInstance() : this(null, null, null)
        {
            
        }

        public RuntimeInstance(Assembly functionAppConfigurationAssembly,
            Action<IServiceCollection, ICommandRegistry> beforeServiceProviderBuild,
            Action<IServiceProvider, ICommandRegistry> afterServiceProviderBuild)
        {
            // Find the configuration implementation and service collection
            IFunctionAppConfiguration configuration = LocateConfiguration(functionAppConfigurationAssembly);
            IContainerProvider containerProvider =
                // ReSharper disable once SuspiciousTypeConversion.Global - externally provided
                (configuration as IContainerProvider) ?? new DefaultContainerProvider();

            ServiceCollection = containerProvider.CreateServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => ServiceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => ServiceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
            );
            
            ICommandRegistry commandRegistry;
            // ReSharper disable once SuspiciousTypeConversion.Global - externally provided
            if (configuration is ICommandingConfigurator commandingConfigurator)
            {
                commandRegistry = commandingConfigurator.AddCommanding(adapter);
            }
            else
            {
                CommandingRuntime commandingRuntime = new CommandingRuntime();
                commandRegistry = commandingRuntime.AddCommanding(adapter);
            }

            // Register internal implementations
            RegisterInternalImplementations();

            // Invoke the builder process
            FunctionHostBuilder builder = CreateBuilderFromConfiguration(commandRegistry, configuration);
            Builder = builder;
            FunctionBuilder functionBuilder = (FunctionBuilder)builder.FunctionBuilder;

            SetupAuthorization(builder, functionBuilder);

            RegisterCoreDependencies(builder.FunctionDefinitions);

            RegisterTimerCommandFactories(builder.FunctionDefinitions);

            RegisterHttpDependencies(builder.FunctionDefinitions);

            RegisterCosmosDependencies(builder.FunctionDefinitions);

            beforeServiceProviderBuild?.Invoke(ServiceCollection, commandRegistry);
            ServiceProvider = containerProvider.CreateServiceProvider(ServiceCollection);
            afterServiceProviderBuild?.Invoke(ServiceProvider, commandRegistry);

            builder.ServiceProviderCreatedAction?.Invoke(ServiceProvider);
            
        }

        private void RegisterCosmosDependencies(
            IReadOnlyCollection<AbstractFunctionDefinition> builderFunctionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in builderFunctionDefinitions)
            {
                if (abstractFunctionDefinition is CosmosDbFunctionDefinition cosmosFunctionDefinition)
                {
                    if (cosmosFunctionDefinition.ErrorHandlerType != null)
                    {
                        types.Add(cosmosFunctionDefinition.ErrorHandlerType);
                    }
                }
            }

            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }
        }

        private void RegisterCoreDependencies(
            IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in functionDefinitions)
            {
                types.Add(abstractFunctionDefinition.CommandDeserializerType);
            }
            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }

            // Inject an ILogger that picks up the runtime provided logger
            ServiceCollection.AddTransient<ILogger>(sp => new FunctionLogger(this));
            ServiceCollection.AddTransient<ILoggerFactory>(sp => new FunctionLoggerFactory(this));
        }

        private void RegisterHttpDependencies(IReadOnlyCollection<AbstractFunctionDefinition> builderFunctionDefinitions)
        {
            HashSet<Type> types = new HashSet<Type>();
            foreach (AbstractFunctionDefinition abstractFunctionDefinition in builderFunctionDefinitions)
            {
                if (abstractFunctionDefinition is HttpFunctionDefinition httpFunctionDefinition)
                {
                    if (httpFunctionDefinition.ClaimsPrincipalAuthorizationType != null)
                    {
                        types.Add(httpFunctionDefinition.ClaimsPrincipalAuthorizationType);
                    }

                    if (httpFunctionDefinition.HttpResponseHandlerType != null)
                    {
                        types.Add(httpFunctionDefinition.HttpResponseHandlerType);
                    }

                    if (httpFunctionDefinition.TokenValidatorType != null)
                    {
                        types.Add(httpFunctionDefinition.TokenValidatorType);
                    }
                }
            }

            foreach (Type claimsPrincipalAuthorizationType in types)
            {
                ServiceCollection.AddTransient(claimsPrincipalAuthorizationType);
            }
        }

        private void SetupAuthorization(FunctionHostBuilder builder, FunctionBuilder functionBuilder)
        {
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder)builder.AuthorizationBuilder;

            // don't register the token validator type here - that gets passed down to the HTTP function
            // definitions to allow for function overrides and so is registered as part of HTTP dependencies
            if (authorizationBuilder.CustomClaimsBinderType == null)
            {
                ICommandClaimsBinder commandClaimsBinder = authorizationBuilder.ClaimsMappingBuilder.Build(
                    functionBuilder.GetHttpFunctionDefinitions().Select(x => x.CommandType).ToArray());
                ServiceCollection.AddSingleton(commandClaimsBinder);
            }
            else
            {
                ServiceCollection.AddTransient(typeof(ICommandClaimsBinder),
                    authorizationBuilder.CustomClaimsBinderType);
            }
        }

        private IFunctionAppConfiguration LocateConfiguration(Assembly functionAppConfigurationAssembly)
        {
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration(functionAppConfigurationAssembly);

            return configuration;
        }

        private FunctionHostBuilder CreateBuilderFromConfiguration(ICommandRegistry commandRegistry,
            IFunctionAppConfiguration configuration)
        {
            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry, true);
            configuration.Build(builder);
            RegisterCommandHandlersForCommandsWithNoAssociatedHandler(builder, commandRegistry);
            new PostBuildPatcher().Patch(builder, "");
            return builder;
        }

        private void RegisterCommandHandlersForCommandsWithNoAssociatedHandler(FunctionHostBuilder builder, ICommandRegistry commandRegistry)
        {
            // IN PROGRESS: This looks from the loaded set of assemblies and looks for a command handler for each command associated with a function.
            // If the handler is not already registered in the command registry then this registers it.
            IRegistrationCatalogue registrationCatalogue = (IRegistrationCatalogue) commandRegistry;
            HashSet<Type> registeredCommandTypes = new HashSet<Type>(registrationCatalogue.GetRegisteredCommands());
            
            Dictionary<Type, List<Type>> commandTypesToHandlerTypes = null;

            foreach (AbstractFunctionDefinition functionDefinition in builder.FunctionDefinitions)
            {
                if (registeredCommandTypes.Contains(functionDefinition.CommandType))
                {
                    continue;
                }

                if (commandTypesToHandlerTypes == null)
                {
                    commandTypesToHandlerTypes = HarvestCommandHandlers();
                }

                if (commandTypesToHandlerTypes.TryGetValue(functionDefinition.CommandType, out List<Type> handlerTypes))
                {
                    foreach (Type handlerType in handlerTypes)
                    {
                        commandRegistry.Register(handlerType);
                    }
                }
            }
        }

        private static Dictionary<Type, List<Type>> HarvestCommandHandlers()
        {
            Type baseCommandHandlerType = typeof(ICommandHandler);
            Dictionary<Type, List<Type>> commandTypesToHandlerTypes = new Dictionary<Type, List<Type>>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type candidateHandlerType in assembly.GetTypes())
                {
                    if (!candidateHandlerType.IsInterface && baseCommandHandlerType.IsAssignableFrom(candidateHandlerType))
                    {
                        Type[] genericArguments = candidateHandlerType.GetInterfaces()
                            .Where(i => i.IsGenericType &&
                                        (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                                         i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                            .SelectMany(i => i.GetGenericArguments())
                            .ToArray();

                        if (genericArguments.Any())
                        {
                            Type commandArgumentType = genericArguments[0];
                            if (!commandTypesToHandlerTypes.TryGetValue(commandArgumentType,
                                out var handlerTypes))
                            {
                                handlerTypes = new List<Type>();
                                commandTypesToHandlerTypes[commandArgumentType] = handlerTypes;
                            }

                            handlerTypes.Add(candidateHandlerType);
                        }
                    }
                }
            }

            return commandTypesToHandlerTypes;
        }

        private void RegisterInternalImplementations()
        {
            ServiceCollection.AddTransient<ICommandClaimsBinder, CommandClaimsBinder>();
            ServiceCollection.AddTransient<IContextSetter, ContextManager>();
            ServiceCollection.AddTransient<IContextProvider, ContextManager>();
        }

        private void RegisterTimerCommandFactories(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
        {
            IReadOnlyCollection<TimerFunctionDefinition> timerFunctionDefinitions = functionDefinitions
                .Where(x => x is TimerFunctionDefinition)
                .Cast<TimerFunctionDefinition>()
                .Where(x => x.TimerCommandFactoryType != null)
                .ToArray();
            foreach (TimerFunctionDefinition timerFunctionDefinition in timerFunctionDefinitions)
            {
                Type interfaceType =
                    typeof(ITimerCommandFactory<>).MakeGenericType(timerFunctionDefinition.CommandType);
                ServiceCollection.AddTransient(interfaceType, timerFunctionDefinition.TimerCommandFactoryType);
            }
        }
    }
}
