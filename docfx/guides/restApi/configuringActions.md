# Configuring Actions

An action can be configured with:

* The command type expressed as a generic parameter
* A HTTP verb (mandatory)
* An optional binding attribute for the command payload e.g. [[FromBody]]. This is expressed as an optional secondary generic parameter. GET and DELETE verbs default to FromRoute while POST and PUT verbs default to FromBody
* An optional route - this is attached to the controller using the [[Route(...)]] attribute.
* An optional set of attribute filters - see Attributes below

A sample illustrating all five of the above options is shown below:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket",
                    actions => actions
                        .Action<GetBasketQuery, FromQueryAttribute>(
                            HttpMethod.Get,
                            "MyBasket",
                            attributes => attributes.Attribute<AuthorizeAttribute>())
                )
            );
    }
