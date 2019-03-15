# HTTP Responses

By default the framework will return a 200 status code and the command response as the body, unless an exceptioon occurs in which case a 500 error will be returned.

The response can be shaped and influenced by registering response handlers against functions. Response handlers must implement the _IHttpResponseHandler_ interface as shown in the example below that will return a 204 no content response when a command result is null:

    public class CustomResponseHandler : IHttpResponseHandler
    {
        // Handle responses for errors
        public Task<IActionResult> CreateResponse<TCommand>(TCommand command, Exception ex) where TCommand : ICommand
        {
            return null;
        }

        // Handle responses for commands with results
        public Task<IActionResult> CreateResponse<TCommand, TResult>(TCommand command, TResult result) where TCommand : ICommand<TResult>
        {
            if (result == null)
            {
                return Task.FromResult((IActionResult)new NoContentResult());
            }

            return null;
        }

        // Handle responses for commands with no results
        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
        {
            return null;
        }
    }

To have Function Monkey handle the response in the default manner simply return either a null task or a task with a null result. In the example above, for example, error handling will be handled as per usual by Function Monkey.

To register this handler on a per function basis use the _ResponseHandler_ method as shown in the example below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>(HttpMethod.Get)
                            .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    )
                );
        }
    }

Alternatively a response handler can be registered as a default handler:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .DefaultHttpResponseHandler<CustomResponseHandler>()
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>(HttpMethod.Get)
                    )
                );
        }
    }

In keeping with Function Monkeys goals of cleanly separating protocol from business logic HTTP response handling is ideally kept separate to command handlers using this method. If you do need to implement response handling from within a command handler then the suggestion is to introduce an interface on a response like the below:

    public interface IHttpResult
    {        
        int StatusCode { get; set; }
    }

And then on a response model implement this as an explicit interface:

    public class MyCommandResponse : IHttpResult
    {
        public string SomeProperty { get; set; }

        int IHttpResult.StatusCode { get; set;}
    }

The response handler can then look for this interface and behave accordingly:

    public class CustomResponseHandler : IHttpResponseHandler
    {
        public Task<IActionResult> CreateResponse<TCommand>(TCommand command, Exception ex) where TCommand : ICommand
        {
            return null;
        }

        public Task<IActionResult> CreateResponse<TCommand, TResult>(TCommand command, TResult result) where TCommand : ICommand<TResult>
        {
            if (result is IHttpResult httpResult)
            {
                return Task.FromResult((IActionResult)new StatusCodeResult(httpResult.StatusCode));
            }

            return null;
        }

        // Handle responses for commands with no results
        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
        {
            return null;
        }
    }

