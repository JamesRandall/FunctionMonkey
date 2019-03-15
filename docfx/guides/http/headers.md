# Headers

## As Context Info

Header values can be accessed through the [HTTP trigger context](/crosscutting/triggercontexts.md).

## Binding

Header values can be automatically bound to command properties by adding a header mapping to the function declaration using the _AddHeaderMapping_ method. In the following example the property OperationId will be mapped from the header x-operation-id:

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
                            .Options(options => options.AddHeaderMapping(x => x.OperationId, "x-operation-id"))
                    )
                );
        }
    }

Additionally default mappings can be configured for all commands using the _DefaultHttpHeaderBindingConfiguration_ method as shown below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .DefaultHttpHeaderBindingConfiguration(new HeaderBindingConfiguration
                {
                    PropertyFromHeaderMappings = new Dictionary<string, string>
                    {
                        { "OperationId", "x-operation-id" }
                    }
                })
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>(HttpMethod.Get)
                    )
                );
        }
    }

In the case that there are multiple values for a header the first will be used.

Header binding occurs after body, route, and query parameter binding.