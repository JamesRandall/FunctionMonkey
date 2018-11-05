# Logging

To add logging support simply register the appropriate dependencies in the _Setup_ method of your builder. For example to add the Microsoft logging framework and make _ILogger_ and _ILogger<T>_ available to your command handlers use a block similar to the below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<HelloWorldCommandHandler>();
                    serviceCollection.AddLogging();
                })
                .Functions(functions => functions
                    .HttpRoute("/api/v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }
