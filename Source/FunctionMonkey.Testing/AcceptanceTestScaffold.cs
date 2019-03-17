using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Model;
using FunctionMonkey.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using HttpResponse = Microsoft.AspNetCore.Http.HttpResponse;
using Microsoft.AspNetCore.Hosting;

namespace FunctionMonkey.Testing
{
    /// <summary>
    /// A scaffold class that can be used to set up Function Monkey acceptance tests designed for test frameworks that
    /// make use of setup and teardown methods.
    /// </summary>
    public class AcceptanceTestScaffold
    {
        private static bool _environmentVariablesRegistered;
        private static readonly object LoadingAppVariablesLock = new object();

        private RuntimeInstance _runtimeInstance;

        // We register our ASP.Net dependencies separately to the Function dependencies as these are (in the current Azure Functions implementation)
        // not shared with function apps.
        private IServiceProvider _aspNetServiceProvider;


        /// <summary>
        /// Setup the scaffold with a IFunctionHostBuilder
        /// </summary>
        /// <param name="beforeBuild">An optional function to run before the Build method is called on the Function App configuration</param>
        /// <param name="afterBuild">An optional function to run before the Build method is called after the Function App configuration</param>
        /// /// <param name="functionAppConfigurationAssembly">If your Function App Configuration cannot be found you may need to provide the assembly it is located within to the setup - this is due to the as needed dependency loader and that a method setup based test may not yet have needed the required assembly.</param>
        public void Setup(
            Assembly functionAppConfigurationAssembly = null,
            Action<IServiceCollection, ICommandRegistry> beforeBuild = null,
            Action<IServiceCollection, ICommandRegistry> afterBuild = null
        )
        {
            SetupAspNet();
            _runtimeInstance = new RuntimeInstance(functionAppConfigurationAssembly, beforeBuild, afterBuild);
        }

        /// <summary>
        /// Add environment variables from a stream containing a Functions app settings file
        /// </summary>
        /// <param name="appSettingsStream">The app settings stream</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public void AddEnvironmentVariables(Stream appSettingsStream, bool oneTimeOnly = true)
        {
            if (_environmentVariablesRegistered && oneTimeOnly)
            {
                return;
            }

            SetEnvironmentVariables(appSettingsStream, oneTimeOnly);
        }

        /// <summary>
        /// Add environment variables from a file containing a Functions app settings file
        /// </summary>
        /// <param name="appSettingsPath">A path to the app settings file</param>
        /// <param name="oneTimeOnly">Defaults to true, if true only set the variables one time</param>
        public void AddEnvironmentVariables(string appSettingsPath, bool oneTimeOnly = true)
        {
            if (_environmentVariablesRegistered && oneTimeOnly)
            {
                return;
            }

            using (Stream stream = new FileStream(appSettingsPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                SetEnvironmentVariables(stream, oneTimeOnly);
            }
        }

        private static void SetEnvironmentVariables(Stream appSettings, bool oneTimeOnly)
        {
            lock (LoadingAppVariablesLock)
            {
                if (_environmentVariablesRegistered && oneTimeOnly)
                {
                    return;
                }

                string json;
                using (StreamReader reader = new StreamReader(appSettings))
                {
                    json = reader.ReadToEnd();
                }

                if (!string.IsNullOrWhiteSpace(json))
                {
                    JObject settings = JObject.Parse(json);
                    JObject values = (JObject) settings["Values"];
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

                _environmentVariablesRegistered = true;
            }

        }

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
            HttpContext httpContext =
                PrepareToExecuteHttp(command, out var actionContext, out var httpFunctionDefinition, out var httpResponseHandler, method);

            IActionResult actionResult = await DispatchAndConvertToActionResult(command, httpResponseHandler, httpFunctionDefinition);            

            await actionResult.ExecuteResultAsync(actionContext);
            
            
            return SanitizeResponse(httpContext.Response);
        }

        /// <summary>
        /// Runs a command through the IActionResult ASP.Net pathways and returns a HTTP response.
        /// This is useful for testing end to end HTTP triggered functions without having to actually host the
        /// function app.
        /// A method only needs specifying in the function supports multiple methods.
        /// </summary>        
        public async Task<HttpResponse> ExecuteHttpAsync(ICommand command, HttpMethod method = null)
        {
            HttpContext httpContext =
                PrepareToExecuteHttp(command, out var actionContext, out var httpFunctionDefinition, out var httpResponseHandler, method);

            IActionResult actionResult = await DispatchAndConvertToActionResult(command, httpResponseHandler, httpFunctionDefinition);

            await actionResult.ExecuteResultAsync(actionContext);

            return SanitizeResponse(httpContext.Response);
        }

        private HttpContext PrepareToExecuteHttp(ICommand command, out ActionContext actionContext,
            out HttpFunctionDefinition httpFunctionDefinition, out IHttpResponseHandler httpResponseHandler,
            HttpMethod method)
        {
            httpFunctionDefinition = FindFunctionDefinition(command.GetType()) as HttpFunctionDefinition;
            if (httpFunctionDefinition == null)
            {
                throw new TestException(
                    $"An http test can only be run for http trigger associated commands, command type {command.GetType()} is not associated with a HTTP trigger");
            }

            HttpContext httpContext = new DefaultHttpContext()
            {
                RequestServices = _aspNetServiceProvider,
                Request =
                {
                    Method = (method ?? httpFunctionDefinition.Verbs.Single()).Method
                },
                Response = { Body = new MemoryStream()}
            };
            RouteData routeData = new RouteData();
            ActionDescriptor actionDescriptor = new ActionDescriptor();
            actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

            httpResponseHandler = httpFunctionDefinition.HasHttpResponseHandler
                ? (IHttpResponseHandler) ServiceProvider.GetRequiredService(httpFunctionDefinition.HttpResponseHandlerType)
                : new DefaultHttpResponseHandler();
            return httpContext;
        }

        private async Task<IActionResult> DispatchAndConvertToActionResult<TResult>(ICommand<TResult> command, IHttpResponseHandler httpResponseHandler,
            HttpFunctionDefinition httpFunctionDefinition)
        {
            IActionResult actionResult = null;
            try
            {
                TResult result = await Dispatcher.DispatchAsync(command);
                // TODO: Handle validation here
                Task<IActionResult> responseHandlerTask = httpResponseHandler.CreateResponse(command, result);
                if (responseHandlerTask != null)
                {
                    actionResult = await responseHandlerTask;
                }

                if (actionResult == null)
                {
                    actionResult = CreateResponse(200, result, httpFunctionDefinition);
                }
            }
            catch (Exception ex)
            {
                Task exceptionResponseHandlerTask = httpResponseHandler.CreateResponseFromException(command, ex);
                if (exceptionResponseHandlerTask != null)
                {
                    actionResult = await httpResponseHandler.CreateResponseFromException(command, ex);
                }

                if (actionResult == null)
                {
                    actionResult = CreateResponse(500, "Unexpected error", httpFunctionDefinition);
                }
            }

            return actionResult;
        }

        private async Task<IActionResult> DispatchAndConvertToActionResult(ICommand command, IHttpResponseHandler httpResponseHandler,
            HttpFunctionDefinition httpFunctionDefinition)
        {
            IActionResult actionResult = null;
            try
            {
                await Dispatcher.DispatchAsync(command);
                // TODO: Handle validation here
                Task<IActionResult> responseHandlerTask = httpResponseHandler.CreateResponse(command);
                if (responseHandlerTask != null)
                {
                    actionResult = await responseHandlerTask;
                }

                if (actionResult == null)
                {
                    actionResult = new OkResult();
                }
            }
            catch (Exception ex)
            {
                Task exceptionResponseHandlerTask = httpResponseHandler.CreateResponseFromException(command, ex);
                if (exceptionResponseHandlerTask != null)
                {
                    actionResult = await httpResponseHandler.CreateResponseFromException(command, ex);
                }

                if (actionResult == null)
                {
                    actionResult = CreateResponse(500, "Unexpected error", httpFunctionDefinition);
                }
            }

            return actionResult;
        }

        private AbstractFunctionDefinition FindFunctionDefinition(Type commandType)
        {
            return _runtimeInstance.Builder.FunctionDefinitions.Single(x => x.CommandType == commandType);
        }

        private IActionResult CreateResponse(int code, object content, HttpFunctionDefinition httpFunctionDefinition)
        {
            ISerializer serializer = CreateSerializer(httpFunctionDefinition);
            ContentResult result = new ContentResult();
            result.Content = serializer.Serialize(content);
            result.ContentType = "application/json";
            result.StatusCode = code;
            return result;
        }

        private ISerializer CreateSerializer(HttpFunctionDefinition httpFunctionDefinition)
        {
            if (httpFunctionDefinition.SerializerNamingStrategyType != null)
            {
                NamingStrategy serializerNamingStrategy = (NamingStrategy)Activator.CreateInstance(httpFunctionDefinition.SerializerNamingStrategyType);
                NamingStrategy deserializerNamingStrategy = (NamingStrategy)Activator.CreateInstance(httpFunctionDefinition.DeserializerNamingStrategyType);
                ISerializer serializer = new NamingStrategyJsonSerializer(deserializerNamingStrategy, serializerNamingStrategy);
                return serializer;
            }

            return (ISerializer)ServiceProvider.GetRequiredService(httpFunctionDefinition.CommandDeserializerType);
        }

        private HttpResponse SanitizeResponse(HttpResponse httpContextResponse)
        {
            httpContextResponse.Body.Seek(0, SeekOrigin.Begin);
            return httpContextResponse;
        }

        private void SetupAspNet()
        {
            IWebHost host =
                Microsoft.AspNetCore.WebHost.CreateDefaultBuilder()
                    .ConfigureServices(sc => { sc.AddMvc(); })
                    .UseStartup<DummyStartup>()
                    .Build();
            _aspNetServiceProvider = host.Services;
        }

        private class DummyStartup : IStartup
        {
            public IServiceProvider ConfigureServices(IServiceCollection services)
            {
                return services.BuildServiceProvider();
            }

            public void Configure(IApplicationBuilder app)
            {
                
            }
        }
    }
}

