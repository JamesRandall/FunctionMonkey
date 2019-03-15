# Serialization

By default Function Monkey is configured to accept JSON using the default (Newtonsoft.JSON) naming strategy for in bound deserialization and the camel case naming strategy for outbound serialization on HTTP requests.

This can be overridden in a number of ways. Firstly the serializer can be replaced in its entirety by implementing the _ISerializer_ interface and registering it as the default serializer as shown in the example below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .Serialization(serialization => serialization.DefaultCommandDeserializer<TMySerializer>())
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>(HttpMethod.Get)
                    )
                );
        }
    }

This can also be set on a per function basis as shown below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .Serialization(serialization => serialization.DefaultCommandDeserializer<TMySerializer>())
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>(HttpMethod.Get)
                            .Options(options => options.Serializer<TMySerializer>())
                    )
                );
        }
    }

Using this approach other formats and schemes can be used - for example XML.

Additionally alternative Newtonsoft.Json naming strategies can be configured on a per function basis. The  below demonstrates using snake case serialization for the return from a HTTP function:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .Serialization(serialization => serialization.DefaultCommandDeserializer<TMySerializer>())
                .Functions(functions => functions
                    .HttpRoute("Invoice", route => route
                        .HttpFunction<InvoiceQuery>(HttpMethod.Get)
                            .Options(options => options.JsonNamingStrategies<DefaultNamingStrategy, SnakeCaseNamingStrategy>())
                    )
                );
        }
    }