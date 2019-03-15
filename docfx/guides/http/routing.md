# Routing

In Function Monkey every HTTP function has a parent route and an optional sub-route. For example to define a parent route of Invoice and add a GET verb based function to that route you would define your configuration block as:

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
                    )
                );
        }
    }

For GET and DELETE based verbs the properties of the commands will, by default, be populated from query parameters. So if we assume our command looks like this:

    public class InvoiceQuery : ICommand<Invoice>
    {
        public int Id { get; set; }
    }

Then we can call it using a URL such as:

    http://localhost:7071/invoice?id=1234

If you want to use path based routing then simply set the sub-route using typical curly brace syntax as shown below:

    .HttpRoute("Invoice", route => route
        .HttpFunction<InvoiceQuery>("/{id}", HttpMethod.Get)

Which will allow you to call the function using a URL such as:

    http://localhost:7071/invoice/1234

Note that route parameters cannot be optional.

In the current version of FunctionMonkey POST and PUT based functions cannot take query parameters but they can take route parameters. The route parameter will take precedence over any value supplied in the request body.