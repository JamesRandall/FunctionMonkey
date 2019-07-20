using System;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.TokenValidator.Implementation;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace FunctionMonkey.TokenValidator
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Adds an OpenID Connect token validator for authorization
    /// </summary>
    public static class IAuthorizationBuilderExtensions
    {
        /// <summary>
        /// Adds an OpenID Connect token validator for authorization
        /// <param name="builder">Builder reference</param>
        /// <param name="wellKnownEndpoint">The well known endpoint</param>
        /// <param name="audience">(Optional) The audience to verify the token is for</param>
        /// <param name="tokenValidationParameterFunc">(Optional) If specified then this function will be called to create the parameters used to validate tokens</param>
        /// <param name="header">(Optional) The header to pull the bearer token from</param>
        /// </summary>
        public static IAuthorizationBuilder AddOpenIdConnectTokenValidator(this IAuthorizationBuilder builder,
            string wellKnownEndpoint,
            string audience=null,
            Func<OpenIdConnectConfiguration, string, TokenValidationParameters> tokenValidationParameterFunc=null,
            string header=null)
        {
            // I didn't want to update the builder interface on the point release this is intended for and wanted
            // to keep the introduction of the package as a none breaking change so it involved a bit of static-ness
            // to work around the existing interface
            Implementation.TokenValidator.Configuration = new TokenValidatorConfiguration(wellKnownEndpoint, audience, tokenValidationParameterFunc);
            builder.TokenValidator<Implementation.TokenValidator>(header);
            return builder;
        }
    }
}