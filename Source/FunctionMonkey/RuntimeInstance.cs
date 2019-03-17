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

        public RuntimeInstance(Assembly functionAppConfigurationAssembly, Action<IServiceCollection, ICommandRegistry> beforeBuild, Action<IServiceCollection, ICommandRegistry> afterBuild)
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

            beforeBuild?.Invoke(ServiceCollection, commandRegistry);

            // Invoke the builder process
            FunctionHostBuilder builder = CreateBuilderFromConfiguration(commandRegistry, configuration);
            Builder = builder;
            FunctionBuilder functionBuilder = (FunctionBuilder)builder.FunctionBuilder;

            SetupAuthorization(builder, functionBuilder);

            RegisterCoreDependencies(builder.FunctionDefinitions);

            RegisterTimerCommandFactories(builder.FunctionDefinitions);

            RegisterHttpDependencies(builder.FunctionDefinitions);

            RegisterCosmosDependencies(builder.FunctionDefinitions);            

            ServiceProvider = containerProvider.CreateServiceProvider(ServiceCollection);
            builder.ServiceProviderCreatedAction?.Invoke(ServiceProvider);

            afterBuild?.Invoke(ServiceCollection, commandRegistry);
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
            ServiceCollection.AddTransient<ILogger, FunctionLogger>();
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

            ICommandClaimsBinder commandClaimsBinder = authorizationBuilder.ClaimsMappingBuilder.Build(
                functionBuilder.GetHttpFunctionDefinitions().Select(x => x.CommandType).ToArray());
            ServiceCollection.AddSingleton(commandClaimsBinder);
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
            new PostBuildPatcher().Patch(builder, "");
            return builder;
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
