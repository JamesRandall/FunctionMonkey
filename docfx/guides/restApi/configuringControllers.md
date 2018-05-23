# Configuring Controllers

A controller can be configured with:

* A name (mandatory)
* An optional route - this is attached to the controller using the [Route(...)] attribute and so if you want the controller name to form part of the route it should include the [controller] component
* An optional set of attribute filters - see Attributes below
* One or more actions - see Actions below

A sample illustrating all four of the above options is shown below:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket",
                    "api/v1/[controller]",
                    attributes => attributes.Attribute<AuthorizeAttribute>(),
                    actions => actions.Action<AddToBasketCommand>(HttpMethod.Post)
                )
            );
    }

### Adding Attributes / Filters
