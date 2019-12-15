using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Model;
using FunctionMonkey.Testing.Implementation;
using FunctionMonkey.Testing.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;

namespace FunctionMonkey.Testing
{
    /// <summary>
    /// A scaffold class that can be used to set up Function Monkey acceptance tests designed for test frameworks that
    /// make use of setup and teardown methods.
    /// </summary>
    public class AcceptanceTestScaffold
    {
        private RuntimeInstance _runtimeInstance;

        private readonly AspNetRuntime _aspNetRuntime = new AspNetRuntime();


        /// <summary>
        /// Setup the scaffold with a IFunctionHostBuilder
        /// </summary>
        /// <param name="beforeServiceProviderBuild">An optional function to run before the Build method is called on the Function App configuration</param>
        /// <param name="afterServiceProviderBuild">An optional function to run before the Build method is called after the Function App configuration</param>
        /// <param name="functionAppConfigurationAssembly">If your Function App Configuration cannot be found you may need to provide the assembly it is located within to the setup - this is due to the as needed dependency loader and that a method setup based test may not yet have needed the required assembly.</param>
        /// <param name="mockLogger">A logger that will be injected into any handlers - handy for supplying a Moq<ILogger> or such like. If not specified a simple internal stub will be used.</param>
        public void Setup(
            Assembly functionAppConfigurationAssembly = null,
            Action<IServiceCollection, ICommandRegistry> beforeServiceProviderBuild = null,
            ILogger mockLogger = null
        )
        {
            _runtimeInstance = new RuntimeInstance(functionAppConfigurationAssembly, beforeServiceProviderBuild, null);

            _runtimeInstance.FunctionProvidedLogger.Value = mockLogger ?? new LoggerMock();
        }

        /// <summary>
        /// Add environment variables from a stream containing a Functions app settings file
        /// </summary>
        /// <param name="appSettingsStream">The app settings stream</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public void AddEnvironmentVariables(Stream appSettingsStream, bool oneTimeOnly = true) =>
            EnvironmentVariableManager.AddEnvironmentVariables(appSettingsStream, oneTimeOnly);

        /// <summary>
        /// Add environment variables from a file containing a Functions app settings file
        /// </summary>
        /// <param name="appSettingsPath">A path to the app settings file</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public void AddEnvironmentVariables(string appSettingsPath, bool oneTimeOnly = true) =>
            EnvironmentVariableManager.AddEnvironmentVariables(appSettingsPath, oneTimeOnly);


        /// <summary>
        /// The constructed service provider
        /// </summary>
        public IServiceProvider ServiceProvider => _runtimeInstance.ServiceProvider;

        /// <summary>
        /// Provides access to the command dispatcher registered in the service provider but wrapped
        /// in a decorator that implements validation.
        /// </summary>
        public ICommandDispatcher Dispatcher
        {
            get
            {
                IValidator validator = ServiceProvider.GetService<IValidator>();
                ICommandDispatcher registeredDispatcher = ServiceProvider.GetService<ICommandDispatcher>();
                ValidatingDispatcher validatingDispatcher = new ValidatingDispatcher(registeredDispatcher, validator);
                return validatingDispatcher;
            }
        }

        /// <summary>
        /// Runs a command through the IActionResult ASP.Net pathways and returns a HTTP response.
        /// This is useful for testing end to end HTTP triggered functions without having to actually host the
        /// function app.
        /// A method only needs specifying in the function supports multiple methods.
        /// </summary>
        public async Task<HttpResponse> ExecuteHttpAsync<TResult>(ICommand<TResult> command, HttpMethod method = null)
        {
            HttpFunctionDefinition httpFunctionDefinition = FindHttpFunctionDefinition(command);
            ActionContext actionContext = _aspNetRuntime.PrepareToExecuteHttp(command, httpFunctionDefinition, method);
            IHttpResponseHandler httpResponseHandler = GetHttpResponseHandler(httpFunctionDefinition);
            HttpDispatcher httpDispatcher = new HttpDispatcher(Dispatcher, ServiceProvider);

            IActionResult actionResult = await httpDispatcher.DispatchAndConvertToActionResult(command, httpResponseHandler, httpFunctionDefinition);

            return await _aspNetRuntime.CreateHttpResponse(actionContext, actionResult);
        }

        /// <summary>
        /// Runs a command through the IActionResult ASP.Net pathways and returns a HTTP response.
        /// This is useful for testing end to end HTTP triggered functions without having to actually host the
        /// function app.
        /// A method only needs specifying in the function supports multiple methods.
        /// </summary>
        public async Task<HttpResponse> ExecuteHttpAsync(ICommand command, HttpMethod method = null)
        {
            HttpFunctionDefinition httpFunctionDefinition = FindHttpFunctionDefinition(command);
            ActionContext actionContext = _aspNetRuntime.PrepareToExecuteHttp(command, httpFunctionDefinition, method);
            IHttpResponseHandler httpResponseHandler = GetHttpResponseHandler(httpFunctionDefinition);
            HttpDispatcher httpDispatcher = new HttpDispatcher(Dispatcher, ServiceProvider);

            IActionResult actionResult = await httpDispatcher.DispatchAndConvertToActionResult(command, httpResponseHandler, httpFunctionDefinition);

            return await _aspNetRuntime.CreateHttpResponse(actionContext, actionResult);
        }

        private IHttpResponseHandler GetHttpResponseHandler(HttpFunctionDefinition httpFunctionDefinition)
        {
            IHttpResponseHandler httpResponseHandler = httpFunctionDefinition.HasHttpResponseHandler
                ? (IHttpResponseHandler)ServiceProvider.GetRequiredService(httpFunctionDefinition.HttpResponseHandlerType)
                : new DefaultHttpResponseHandler();
            return httpResponseHandler;
        }

        private HttpFunctionDefinition FindHttpFunctionDefinition(ICommand command)
        {
            AbstractFunctionDefinition functionDefinition = _runtimeInstance.FunctionDefinitions.Single(x => x.CommandType == command.GetType());
            if (!(functionDefinition is HttpFunctionDefinition httpFunctionDefinition))
            {
                throw new TestException(
                    $"An http test can only be run for http trigger associated commands, command type {command.GetType().Name} is not associated with a HTTP trigger");
            }

            return httpFunctionDefinition;
        }
    }
}

