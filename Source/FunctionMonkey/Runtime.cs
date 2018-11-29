using System;
using System.Collections.Generic;
using System.Linq;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Builders;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey
{
    /// <summary>
    /// This class provides a host for dependency resolver and access to the command dispatcher if it is needed
    /// from standard functions.
    /// 
    /// The first time it is accessed (typically by the first function call) its static constructor will kick in
    /// and it will create the necessary environment for the functions based on the Setup() block of the function
    /// app configuration.
    /// </summary>
    public static class Runtime
    {
        /// <summary>
        /// The dependency resolver
        /// </summary>
        public static readonly IServiceProvider ServiceProvider;

        private static readonly IServiceCollection ServiceCollection;

        static Runtime()
        {
            // Find the configuration implementation and service collection
            IFunctionAppConfiguration configuration = LocateConfiguration();
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
                commandRegistry = adapter.AddCommanding();
            }

            // Register internal implementations
            RegisterInternalImplementations();

            // Invoke the builder process
            FunctionHostBuilder builder = CreateBuilderFromConfiguration(commandRegistry, configuration);
            FunctionBuilder functionBuilder = (FunctionBuilder) builder.FunctionBuilder;

            SetupAuthorization(builder, functionBuilder);

            RegisterCoreDependencies(builder.FunctionDefinitions);

            RegisterTimerCommandFactories(builder.FunctionDefinitions);

            RegisterHttpDependencies(builder.FunctionDefinitions);

            RegisterCosmosDependencies(builder.FunctionDefinitions);
            
            ServiceProvider = containerProvider.CreateServiceProvider(ServiceCollection);
            builder.ServiceProviderCreatedAction?.Invoke(ServiceProvider);            
        }

        private static void RegisterCosmosDependencies(
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

        private static void RegisterCoreDependencies(
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
        }

        private static void RegisterHttpDependencies(IReadOnlyCollection<AbstractFunctionDefinition> builderFunctionDefinitions)
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

        private static void SetupAuthorization(FunctionHostBuilder builder, FunctionBuilder functionBuilder)
        {
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder) builder.AuthorizationBuilder;
            
            // don't register the token validator type here - that gets passed down to the HTTP function
            // definitions to allow for function overrides and so is registered as part of HTTP dependencies

            ICommandClaimsBinder commandClaimsBinder = authorizationBuilder.ClaimsMappingBuilder.Build(
                functionBuilder.GetHttpFunctionDefinitions().Select(x => x.CommandType).ToArray());
            ServiceCollection.AddSingleton(commandClaimsBinder);
        }

        private static IFunctionAppConfiguration LocateConfiguration()
        {
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration();
            
            return configuration;
        }

        private static FunctionHostBuilder CreateBuilderFromConfiguration(ICommandRegistry commandRegistry,
            IFunctionAppConfiguration configuration)
        {
            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry, true);
            configuration.Build(builder);
            new PostBuildPatcher().Patch(builder, "");
            return builder;
        }

        private static void RegisterInternalImplementations()
        {
            ServiceCollection.AddTransient<ICommandClaimsBinder, CommandClaimsBinder>();
            ServiceCollection.AddTransient<IContextSetter, ContextManager>();
            ServiceCollection.AddTransient<IContextProvider, ContextManager>();
        }

        private static void RegisterTimerCommandFactories(IReadOnlyCollection<AbstractFunctionDefinition> functionDefinitions)
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

        /// <summary>
        /// Retrieves the command dispatcher from the dependency resolver
        /// </summary>
        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
