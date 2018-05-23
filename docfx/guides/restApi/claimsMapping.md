# Claims Mapping

Properties on commands can be set by mapping the values from claims. If we consider our previous example:

    public class AddToBasketCommand : ICommand
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }
    }

It is quite likely that the value for the UserId property will actually be found in a claim. If you stick to a convention based approach (e.g. all user IDs on all commands are called UserId) then a single claim mapping can deal with all commands that contain a UserID property. Assuming our claim containing the user ID is called UniqueUserId (I've just differentiated the name to make the example clear) then this can be setup as shown in the example below:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<AddToBasketCommand>(HttpMethod.Post))
                .Claims(mapping => mapping.MapClaimToPropertyName("UniqueUserId", "UserId))
            );
    }

If you have some commands that don't follow the pattern then the generic claim mapping can be overridden by setting up a mapping for a specific command type. For example lets assume our GetBasketQuery has a property called BasketUserId:

    public class GetBasketQuery : ICommand<Basket>
    {
        [SecurityProperty]
        public Guid BasketUserId { get; set; }
    }

Then we can set up a specific mapping for the GetBasketQuery command as follows:

    public void ConfigureServices(IServiceCollection services)
    {
        // ... normal setup of commanding and other infrastructure

        services
            .AddMvc()
            .AddAspNetCoreCommanding(cfg => cfg
                .Controller("Basket", controller => controller
                    .Action<AddToBasketCommand>(HttpMethod.Post))
                .Claims(mapping => mapping
                    .MapClaimToPropertyName("UniqueUserId", "UserId)
                    .MapClaimToCommandProperty<GetBasketQuery>("UserId", cmd => cmd.BasketUserId))
            );
    }

Claims are mapped during model binding and so are present when validation takes place.
