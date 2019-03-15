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
                    .HttpRoute("v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }

That will create a specification at https://YOURDOMAIN/ROUTEPREFIX/openapi.yaml and a user interface explorer at https://YOURDOMAIN/ROUTEPREFIX/openapi

The route prefix is applied across all routes by the Azure Functions runtime and defaults to the workd _api_ so your Open API support will actually appear at https://YOURDOMAIN/api/openapi.yaml and https://YOURDOMAIN/api/openapi.

If you don't want this behaviour you can remove the route prefix entirely by editing the host.json file to read:

    {
        "version": "2.0",
        "extensions": {
            "http": {
                "routePrefix": ""
            }
        }
    }


## Additional Documentation

Function Monkey supports additional OpenAPI documentation to be provided at both the route and function level.

At the route level FunctionMonkey will default to giving the route the same OpenAPI name (in an OpenAPI tag) as the last major route component. So in a route of:

    .HttpRoute("v1/HelloWorld/{text}" ... )

Function Monkey will give the route a name of HelloWorld. A description can be supplied and the name overriden as shown below:

    .HttpRoute("v1/HelloWorld/{text}" ... )
        .OpenApiDescription("Methods on this route represent the MyHelloWorld resource")
        .OpenApiName("MyHelloWorld")

At the Function level descriptions and response documentation can also be set:

    .HttpRoute("v1/HelloWorld", functions => functions
        .HttpFunction<HelloWorldCommand>()
            .OpenApiDescription("Says hello world")
            .OpenApiResponse(400, "An error in the inputs")
    )

In general its best to leave FunctionMonkey to generate the response documentation for the 200 code while annotating any other responses as appropriate.