using System;
using System.Linq;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Builders;
using FunctionMonkey.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey
{
    public static class Runtime
    {
        public static readonly IServiceProvider ServiceProvider;
        private static readonly IServiceCollection ServiceCollection;

        static Runtime()
        {
            ServiceCollection = new ServiceCollection();
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => ServiceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => ServiceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
            );

            // Find the configuration implementation
            ICommandRegistry commandRegistry;
            IFunctionAppConfiguration configuration = ConfigurationLocator.FindConfiguration();
            if (configuration is ICommandingConfigurator commandingConfigurator)
            {
                commandRegistry = commandingConfigurator.AddCommanding(adapter);
            }
            else
            {
                commandRegistry = adapter.AddCommanding();
            }

            // Register internal implementations
            ServiceCollection.AddTransient<ICommandClaimsBinder, CommandClaimsBinder>();
            ServiceCollection.AddTransient<ICommandDeserializer, CommandDeserializer>();
            ServiceCollection.AddTransient<IContextSetter, ContextManager>();
            ServiceCollection.AddTransient<IContextProvider, ContextManager>();

            // Invoke the builder process
            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry);
            configuration.Build(builder);
            new PostBuildPatcher().Patch(builder, "");

            FunctionBuilder functionBuilder = (FunctionBuilder) builder.FunctionBuilder;
            AuthorizationBuilder authorizationBuilder = (AuthorizationBuilder) builder.AuthorizationBuilder;
            if (authorizationBuilder.TokenValidatorType != null)
            {
                ServiceCollection.AddTransient(typeof(ITokenValidator), authorizationBuilder.TokenValidatorType);
            }

            ICommandClaimsBinder commandClaimsBinder = authorizationBuilder.ClaimsMappingBuilder.Build(
                functionBuilder.GetHttpFunctionDefinitions().Select(x => x.CommandType).ToArray());
            ServiceCollection.AddSingleton(commandClaimsBinder);
            
            ServiceProvider = ServiceCollection.BuildServiceProvider();

            builder.ServiceProviderCreatedAction?.Invoke(ServiceProvider);
        }

        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
