using System;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace FunctionMonkey.TokenValidator.Implementation
{
    internal class TokenValidatorConfiguration
    {
        public TokenValidatorConfiguration(string wellKnownEndpoint,
            string audience=null,
            Func<OpenIdConnectConfiguration, string, TokenValidationParameters> tokenValidationParameterFunc=null)
        {
            //string wellKnownEndpoint = $"https://{domain}/.well-known/openid-configuration";
            var documentRetriever = new HttpDocumentRetriever { RequireHttps = wellKnownEndpoint.StartsWith("https://") };
            ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                wellKnownEndpoint,
                new OpenIdConnectConfigurationRetriever(),
                documentRetriever
            );
            Audience = audience;
            TokenValidationParameterFunc = tokenValidationParameterFunc ?? ((config, audienceParam) =>
                                               new TokenValidationParameters()
                                               {
                                                   RequireSignedTokens = true,
                                                   ValidAudience = audienceParam,
                                                   ValidateAudience = !string.IsNullOrEmpty(audienceParam),
                                                   ValidIssuer = config.Issuer,
                                                   ValidateIssuer = true,
                                                   ValidateIssuerSigningKey = true,
                                                   ValidateLifetime = true,
                                                   IssuerSigningKeys = config.SigningKeys
                                               });
        }
        
        public IConfigurationManager<OpenIdConnectConfiguration> ConfigurationManager { get; }
        
        public Func<OpenIdConnectConfiguration, string, TokenValidationParameters> TokenValidationParameterFunc { get; }
        
        public string Audience { get; }
    }
}