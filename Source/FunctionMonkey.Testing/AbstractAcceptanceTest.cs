using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Testing
{
    /// <summary>
    /// A class that can be used as a basis for running acceptance tests with Function Monkey at the command dispatch level
    /// designed for use with test frameworks that take a constructor approach to 
    /// 
    /// One of the advantages of the pattern used by Function Monkey is that the host function triggers are separated cleanly
    /// from business logic and compiled with pre-tested templates allowing for comprehensive acceptance tests to be run
    /// just below this level which can often provide a high level of value with a lower level of complexity than also testing
    /// the Function triggers.
    ///
    /// Typically the none-generic version of this class is more useful but this version allows for a custom IFunctionHostBuilder
    /// to be used if additional functionality is required over and above that baked into the supplied TestFunctionHostBuilder which
    /// solely handles command registration and dependency injection management.
    /// </summary>
    public abstract class AbstractAcceptanceTest
    {
        private readonly AcceptanceTestScaffold _scaffold = new AcceptanceTestScaffold();

        protected AbstractAcceptanceTest()
        {
            _scaffold.Setup(null, BeforeBuild, AfterBuild);
        }

        /// <summary>
        /// Set up environment variables based on a settings.json file in a stream
        /// </summary>
        // ReSharper disable once UnusedMember.Global - intended for use in derived classes in external assemblies
        protected void AddEnvironmentVariables(Stream appSettings, bool oneTimeOnly=true)
        {
            _scaffold.AddEnvironmentVariables(appSettings, oneTimeOnly);
        }

        /// <summary>
        /// Set up environment variables based on a settings.json filename
        /// </summary>
        // ReSharper disable once UnusedMember.Global - intended for use in derived classes in external assemblies
        protected void AddEnvironmentVariables(string appSettingsPath, bool oneTimeOnly = true)
        {
            _scaffold.AddEnvironmentVariables(appSettingsPath, oneTimeOnly);
        }

        /// <summary>
        /// This method can be used to modify dependency and command setup before the Function App Configuration
        /// builder has been run and before tests are run.
        /// 
        /// This must not access members (and should not need to) as it is invoked from the constructor to
        /// support test frameworks such as XUnit that construct test cases this way.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="commandRegistry"></param>
        public virtual void BeforeBuild(IServiceCollection serviceCollection, ICommandRegistry commandRegistry)
        {

        }

        /// <summary>
        /// This method can be used to modify dependency and command setup after the Function App Configuration
        /// builder has been run and before tests are run.
        /// 
        /// This must not access members (and should not need to) as it is invoked from the constructor to
        /// support test frameworks such as XUnit that construct test cases this way.
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="commandRegistry"></param>
        public virtual void AfterBuild(IServiceCollection serviceCollection, ICommandRegistry commandRegistry)
        {

        }

        /// <summary>
        /// The constructed service provider
        /// </summary>
        // ReSharper disable once UnusedMember.Global - intended for use in derived classes in external assemblies
        public IServiceProvider ServiceProvider => _scaffold.ServiceProvider;

        /// <summary>
        /// Provides access to the command dispatcher registered in the service provider but wrapped
        /// in a decorator that implements validation.
        /// </summary>
        // ReSharper disable once UnusedMember.Global - intended for use in derived classes in external assemblies
        public ICommandDispatcher Dispatcher => _scaffold.Dispatcher;

        /// <summary>
        /// Runs a command through the IActionResult ASP.Net pathways and returns a HTTP response.
        /// This is useful for testing end to end HTTP triggered functions without having to actually host the
        /// function app.
        /// A method only needs specifying in the function supports multiple methods.
        /// </summary>        
        public Task<HttpResponse> ExecuteHttpAsync<TResult>(ICommand<TResult> command, HttpMethod method = null) => _scaffold.ExecuteHttpAsync(command, method);

        /// <summary>
        /// Runs a command through the IActionResult ASP.Net pathways and returns a HTTP response.
        /// This is useful for testing end to end HTTP triggered functions without having to actually host the
        /// function app.
        /// A method only needs specifying in the function supports multiple methods.
        /// </summary>
        public Task<HttpResponse> ExecuteHttpAsync(ICommand command, HttpMethod method = null) => _scaffold.ExecuteHttpAsync(command, method);
    }
}
