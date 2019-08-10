using System;
using System.Collections.Generic;
using System.Threading;
using AzureFromTheTrenches.Commanding.Abstractions;
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
        public static IServiceProvider ServiceProvider => RuntimeInstance.ServiceProvider;

        public static AsyncLocal<ILogger> FunctionProvidedLogger => RuntimeInstance.FunctionProvidedLogger;

        public static Dictionary<string, PluginFunctions> PluginFunctions => RuntimeInstance.PluginFunctions;

        private static readonly RuntimeInstance RuntimeInstance;

        static Runtime()
        {
            RuntimeInstance = new RuntimeInstance();      
        }
        

        /// <summary>
        /// Retrieves the command dispatcher from the dependency resolver
        /// </summary>
        public static ICommandDispatcher CommandDispatcher => ServiceProvider.GetService<ICommandDispatcher>();
    }
}
