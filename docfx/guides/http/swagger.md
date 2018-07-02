# Swagger / OpenAPI

If you're building HTTP APIs it can be useful to provide an [OpenAPI](https://www.openapis.org/) (previously known as Swagger) specification and API explorer for developers.

Function Monkey lets you quickly and easily add an API and accompanying documentation and will bake into the compiled Azure Function output everything needed to serve it up.

To enable basic output simply add the _.OpenApiEndpoint_ block as shown in the example below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("My API Title")
                    .Version("0.0.0")
                    .UserInterface()
                )
                .Functions(functions => functions
                    .HttpRoute("/api/v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

That will create a specification at https://YOURDOMAIN/openapi.yaml and a user interface explorer at https://YOURDOMAIN/openapi

