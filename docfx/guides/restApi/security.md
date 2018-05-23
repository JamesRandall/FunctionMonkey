# Security

When ASP.Net Core is configured with this framework the standard authentication and authorization mechanisms continue to apply.

However by exposing commands as REST endpoints we need to take care not to expose any sensitive properies as commands may may contain properties that you never want a caller to be able to set either via a query string, route parameter or body as doing so may cause unwanted side effects such as a data breach. Often such properties need to be populated from claims rather than by caller modifiable data.

A good example might be a user ID. If we consider our earlier AddToBasketCommand command in order for the product to be added to the correct basket it is likely that the command handler and downstream systems will need to be aware of which users basket we are dealing with and so a more realistic representation of this class might be:

    public class AddToBasketCommand : ICommand
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }
    }

By marking the UserId property with the attribute [SecurityProperty] that property cannot be set by sending data to the API endpoint.

Additionally by adding and configuring the AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle package the properties will never be shown in the Swagger definition. The package and Swashbuckle are configured together in ConfigureServices by using the AddSwaggerGen options:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        // ... configure REST API commanding

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            c.AddAspNetCoreCommanding();
        });
    }