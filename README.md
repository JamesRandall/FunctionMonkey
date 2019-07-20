# Function Monkey

Write more elegant Azure Functions with less boilerplate, more consistency, and support for REST APIs.

```C#
public class FunctionAppConfiguration : IFunctionAppConfiguration
{
    public void Build(IFunctionHostBuilder builder)
    {
        builder
            .Setup((serviceCollection, commandRegistry) =>
            {
                serviceCollection
                    .AddLogging()
                    .AddNotificationServices(commandRegistry)
                    .AddExpensesService(commandRegistry)
                    .AddInvoiceServices(commandRegistry);
            })
            .Authorization(authorization => authorization.TokenValidator())
            .AddFluentValidation()
            .Functions(functions => functions
                .HttpRoute("v1/Invoice", route => route
                    .HttpFunction(AuthorizationTypeEnum.TokenValidation, HttpMethod.Get)
                )
                .ServiceBus("serviceBusConnection", serviceBus => serviceBus
                    .SubscriptionFunction("emaildispatchtopic", "emaildispatchsubscription"))
                .Storage("storageConnectionString", storage => storage
                    .BlobFunction("expenses/{name}"))
            );
    }
}
```

## Getting Started

Full documentation is available here:

https://functionmonkey.azurefromthetrenches.com

In addition there is a video tutorial series on building out a simple application:

[![Video tutorial series](https://github.com/JamesRandall/FunctionMonkey/blob/master/videoScreenshot.png?raw=true)](https://www.youtube.com/playlist?list=PLB09mElO-eDipUqGup9d8GFCL2KUj1JYx "Video tutorial series")

https://www.youtube.com/playlist?list=PLB09mElO-eDipUqGup9d8GFCL2KUj1JYx
