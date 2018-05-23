# ASP.Net Core REST API Quickstart

The framework provides an extension package (AzureFromTheTrenches.Commanding.AspNetCore) that allows commands to be exposed as REST endpoints using a configuration based approach removing all the boilerplate associated with implementing controllers and helping to enforce a consistent implementation and API expression.

An example solution can be found at the link below: 

[https://github.com/JamesRandall/AzureFromTheTrenches.Commanding-Quickstarts/tree/master/Source/RestApi](https://github.com/JamesRandall/AzureFromTheTrenches.Commanding-Quickstarts/tree/master/Source/RestApi)

## Installation

Add the NuGet package to your ASP.Net Core project:

    Install-Package AzureFromTheTrenches.Commanding.AspNetCore

If you're using Swashbuckle and your commands contain sensitive properties (see Protecting Sensitive Properties below) it is recommended that you also add the Swashbuckle package - your properties will be secure and tamper proof without this package but adding this package and configuring it will stop those properties from being exposed in the Swagger document:

    Install-Package AzureFromTheTrenches.Commanding.AspNetCore.Swashbuckle


## Configuration

Controllers and actions are mapped to commands by an extension method on the IMvcBuilder returned from the .AddMvc() method that in the typical ASP.Net Core startup project can be found in ConfigureServices.

Let's assume we have a command like the below:

    public class AddToBasketCommand : ICommand
    {
        public Guid ProductId { get; set; }
    }

And we want to expose this command as a POST request on the route /api/Basket then a simple configuration block for this might look like:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<AddToBasketCommand>(HttpMethod.Post)
                )
            );
    }

Although other configuration options exist that's all that is needed to map a command to an endpoint using the default conventions. The configuration API is fluent so we can add a GetBaskketQuery command as follows:

    public class GetBasketQuery : ICommand<Basket>
    {

    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<GetBasketQuery>(HttpMethod.Get)
                    .Action<AddToBasketCommand>(HttpMethod.Post)
                )
            );
    }

And we can add another controller to get the users profile in a similar way:

    public class GetUserProfileQuery : ICommand<UserProfile>
    {

    }

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<GetBasketQuery>(HttpMethod.Get)
                    .Action<AddToBasketCommand>(HttpMethod.Post)
                )
                .Controller("UserProfile", controller => controller
                    .Action<GetUserProfileQuery>(HttpMethod.Get)
                )
            );
    }

## Cleanup

With the above in place you can delete the Controllers folder - all you need is Startup.cs and Program.cs.