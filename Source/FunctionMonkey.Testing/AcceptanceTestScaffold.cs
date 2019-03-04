using FunctionMonkey.Testing.Mocks;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using AzureFromTheTrenches.Commanding;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace FunctionMonkey.Testing
{
    /// <summary>
    /// A scaffold class that can be used to set up Function Monkey acceptance tests designed for test frameworks that
    /// make use of setup and teardown methods.
    /// </summary>
    public class AcceptanceTestScaffold
    {
        private static int _environmentVariablesRegistered = 0;

        /// <summary>
        /// Setup the scaffold with the default TestFunctionHostBuilder
        /// </summary>
        /// <param name="beforeBuild">An optional function to run before the Build method is called on the Function App configuration</param>
        /// <param name="afterBuild">An optional function to run before the Build method is called after the Function App configuration</param>
        /// <param name="functionAppConfigurationAssembly">If your Function App Configuration cannot be found you may need to provide the assembly it is located within to the setup - this is due to the as needed dependency loader and that a method setup based test may not yet have needed the required assembly.</param>
        public void Setup(
            Assembly functionAppConfigurationAssembly = null,
            Action<IServiceCollection, ICommandRegistry> beforeBuild = null,
            Action<IServiceCollection, ICommandRegistry> afterBuild = null)
        {
            Setup<TestFunctionHostBuilder>(functionAppConfigurationAssembly, beforeBuild, afterBuild);
        }

        /// <summary>
        /// Setup the scaffold with a IFunctionHostBuilder
        /// </summary>
        /// <param name="beforeBuild">An optional function to run before the Build method is called on the Function App configuration</param>
        /// <param name="afterBuild">An optional function to run before the Build method is called after the Function App configuration</param>
        /// /// <param name="functionAppConfigurationAssembly">If your Function App Configuration cannot be found you may need to provide the assembly it is located within to the setup - this is due to the as needed dependency loader and that a method setup based test may not yet have needed the required assembly.</param>
        public void Setup<TFunctionHostBuilder>(
            Assembly functionAppConfigurationAssembly = null,
            Action<IServiceCollection, ICommandRegistry> beforeBuild = null,
            Action<IServiceCollection, ICommandRegistry> afterBuild = null
            )
            where TFunctionHostBuilder : class, IFunctionHostBuilder
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            RegisterFunctionMonkeyMocks(serviceCollection);
            CommandingDependencyResolverAdapter adapter = new CommandingDependencyResolverAdapter(
                (fromType, toInstance) => serviceCollection.AddSingleton(fromType, toInstance),
                (fromType, toType) => serviceCollection.AddTransient(fromType, toType),
                (resolveType) => ServiceProvider.GetService(resolveType)
            );

            // We register the commanding runtime on a per test / thread basis rather than globally
            // as multiple tests running concurrently could be undertaking setup at the same time
            // (we want this to be isolate)
            CommandingRuntime commandingRuntime = new CommandingRuntime();
            ICommandRegistry commandRegistry = commandingRuntime.AddCommanding(adapter);

            IFunctionAppConfiguration functionAppConfiguration = functionAppConfigurationAssembly != null
                ? ConfigurationLocator.FindConfiguration(functionAppConfigurationAssembly)
                : ConfigurationLocator.FindConfiguration();

            IFunctionHostBuilder testFunctionHostBuilder =
                (TFunctionHostBuilder) Activator.CreateInstance(
                    typeof(TFunctionHostBuilder),
                    BindingFlags.Default,
                    null,
                    new object[] {serviceCollection, commandRegistry},
                    null);

            beforeBuild?.Invoke(serviceCollection, commandRegistry);

            functionAppConfiguration.Build(testFunctionHostBuilder);

            afterBuild?.Invoke(serviceCollection, commandRegistry);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            Dispatcher = ServiceProvider.GetService<ICommandDispatcher>();
        }

        /// <summary>
        /// Registers the internal dependencies of the Function Monkey runtime
        /// </summary>
        /// <param name="serviceCollection"></param>
        protected virtual void RegisterFunctionMonkeyMocks(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICommandClaimsBinder, CommandClaimsBinderMock>();
            serviceCollection.AddTransient<IContextSetter, ContextManagerMock>();
            serviceCollection.AddTransient<IContextProvider, ContextManagerMock>();
        }

        /// <summary>
        /// Add environment variables from a stream containing a Functions appsettings file
        /// </summary>
        /// <param name="appSettingsStream">The app settings stream</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public void AddEnvironmentVariables(Stream appSettingsStream, bool oneTimeOnly = true)
        {
            if (Interlocked.Exchange(ref _environmentVariablesRegistered, 1) == 1)
            {
                return;
            }

            SetEnvironmentVariables(appSettingsStream);
        }

        /// <summary>
        /// Add environment variables from a file containing a Functions appsettings file
        /// </summary>
        /// <param name="appSettingsPath">A path to the app settings file</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public void AddEnvironmentVariables(string appSettingsPath, bool oneTimeOnly = true)
        {
            if (Interlocked.Exchange(ref _environmentVariablesRegistered, 1) == 1)
            {
                return;
            }

            using (Stream stream = new FileStream(appSettingsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                SetEnvironmentVariables(stream);
            }
        }

        private static void SetEnvironmentVariables(Stream appSettings)
        {
            string json;
            using (StreamReader reader = new StreamReader(appSettings))
            {
                json = reader.ReadToEnd();
            }

            if (!string.IsNullOrWhiteSpace(json))
            {
                JObject settings = JObject.Parse(json);
                JObject values = (JObject)settings["Values"];
                if (values != null)
                {
                    foreach (JProperty property in values.Properties())
                    {
                        if (property.Value != null)
                        {
                            Environment.SetEnvironmentVariable(property.Name, property.Value.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The constructed service provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// A convenience property to provide easy access to the registered ICommandDispatcher
        /// </summary>
        public ICommandDispatcher Dispatcher { get; private set; }
    }
}

