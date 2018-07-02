# Claims Mapping

When using token based authorization claims can be set on commands based on the claims principal user set during that process. Generally these would be marked with the _SecurityPropertyAttribute_ as shown in the example below:

    public class AddToBasketCommand : ICommand
    {
        [SecurityProperty]
        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }
    }

Marking properties with the _SecurityPropertyAttribute_ will prevent them from being populated via payloads, query parameters or route parameters.

If you stick to a convention based approach (e.g. all user IDs on all commands are called UserId) then a single claim mapping can deal with all commands that contain a UserID property. Assuming our claim containing the user ID is called UniqueUserId (I've just differentiated the name to make the example clear) then this can be setup as shown in the example below:

    .Authorization(authorization => authorization
        .AuthorizationDefault(AuthorizationTypeEnum.TokenValidation)
        .TokenValidator<BearerTokenValidator>()
        .Claims(mapping => mapping
            .MapClaimToPropertyName("UniqueUserId", "UserId")            
        )
    )

If you have some commands that don't follow the pattern then the generic claim mapping can be overridden by setting up a mapping for a specific command type. For example lets assume a GetBasketQuery has a property called BasketUserId:

    public class GetBasketQuery : ICommand<Basket>
    {
        [SecurityProperty]
        public Guid BasketUserId { get; set; }
    }

Then we can set up a specific mapping for the GetBasketQuery command as follows:

    .Authorization(authorization => authorization
        .AuthorizationDefault(AuthorizationTypeEnum.TokenValidation)
        .TokenValidator<BearerTokenValidator>()
        .Claims(mapping => mapping
            .MapClaimToPropertyName("UniqueUserId", "UserId")
            .MapClaimToCommandProperty<GetBasketQuery>("UniqueUserId", command => command.BaskerUserId)
        )
    )

Claims are mapped before validation and so can properties derived from them can take part in validation.
