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
            FunctionHostBuilder builder = new FunctionHostBuilder(ServiceCollection, commandRegistry, true);
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

        /// <summary>
        /// Retrieves the command dispatcher from the dependency resolver
        /// </summary>
        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
