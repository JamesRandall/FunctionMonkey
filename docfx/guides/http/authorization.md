# Authorization

Function Monkey supports the standard authorization types of Azure Functions and adds support for token validation through the Authorization header - typically for use with OpenID Connect and an access token. If you're using token validation then you need to register a class that is able to verify the token and populate a ClaimsPrincipal.

The authorization type can be specified per function and a default can be set. In the example below token validation is set as a default, a token validator is registered while one of the functions is set to use anonymous authorization:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<InvoiceQueryHandler>();
                })
                .Authorization(authorization => authorization
                    .AuthorizationDefault(AuthorizationTypeEnum.TokenValidation)
                    .TokenValidator<BearerTokenValidator>()
                )
                .Functions(functions => functions
                    .HttpRoute("/Invoice", route => route
                        .HttpFunction<InvoiceQuery>()
                    )
                    .HttpRoute("/Version", route => route
                        .HttpFunction<VersionQuery>(AuthorizationTypeEnum.Anonymous))
                );
        }
    }

Validators should implement the _ITokenValidator_ interface as shown in the example below (also available as a [gist](https://gist.github.com/JamesRandall/e83f72f98bde2f6ff973e6ecb81199c8)):

    public class BearerTokenValidator : ITokenValidator
    {
        private static readonly IConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager;

        static BearerTokenValidator()
        {
            string domain = Environment.GetEnvironmentVariable("domain");
            
            string wellKnownEndpoint = $"https://{domain}/.well-known/openid-configuration";
            var documentRetriever = new HttpDocumentRetriever { RequireHttps = wellKnownEndpoint.StartsWith("https://") };
            ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                wellKnownEndpoint,
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
        }

        public async Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader)
        {
            if (!authorizationHeader.StartsWith("Bearer "))
                return null;
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);

            var config = await ConfigurationManager.GetConfigurationAsync(CancellationToken.None);
            var audience = Environment.GetEnvironmentVariable("audience");

            var validationParameter = new TokenValidationParameters()
            {
                RequireSignedTokens = true,
                ValidAudience = audience,
                ValidateAudience = true,
                ValidIssuer = config.Issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKeys = config.SigningKeys
            };

            ClaimsPrincipal result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    result = handler.ValidateToken(bearerToken, validationParameter, out SecurityToken _);
                }
                catch (SecurityTokenSignatureKeyNotFoundException)
                {
                    // This exception is thrown if the signature key of the JWT could not be found.
                    // This could be the case when the issuer changed its signing keys, so we trigger a 
                    // refresh and retry validation.
                    ConfigurationManager.RequestRefresh();
                    tries++;
                }
                catch (SecurityTokenException)
                {
                    return null;
                }
            }

            return result;
        }
    }

