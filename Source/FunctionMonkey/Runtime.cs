using System;
using System.Collections.Generic;
using System.Threading;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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
        public static IServiceProvider ServiceProvider => RuntimeInstance.Value.ServiceProvider;

        public static AsyncLocal<ILogger> FunctionProvidedLogger => RuntimeInstance.Value.FunctionProvidedLogger;

        public static Dictionary<string, PluginFunctions> PluginFunctions => RuntimeInstance.Value.PluginFunctions;
        
        public static AsyncLocal<IServiceProvider> FunctionServiceProvider => RuntimeInstance.Value.FunctionServiceProvider;

        private static readonly Lazy<RuntimeInstance> RuntimeInstance =
            new Lazy<RuntimeInstance>(() =>
                new RuntimeInstance(null, null, ServiceCollection));
        
        private static IServiceCollection ServiceCollection { get; set; }

        public static void InitializeFromStartup(IServiceCollection serviceCollection)
        {
            ServiceCollection = serviceCollection;
            var _ = RuntimeInstance.Value;
        }

        public static IMediatorDecorator Mediator => ServiceProvider.GetService<IMediatorDecorator>();
    }
}
