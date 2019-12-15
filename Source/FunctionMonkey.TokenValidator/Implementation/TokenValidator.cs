using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FunctionMonkey.Abstractions;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace FunctionMonkey.TokenValidator.Implementation
{
    // Needs to be public as its created by concrete type reference in the templates. I need to improve this in v4
    public class TokenValidator : ITokenValidator
    {
        internal static TokenValidatorConfiguration Configuration = null;

        public async Task<ClaimsPrincipal> ValidateAsync(string authorizationHeader)
        {
            if (Configuration == null)
            {
                throw new ConfigurationException("Token validator has been incorrectly installed. Please use AddOpenIdConnectTokenValidator");
            }
            if (!authorizationHeader.StartsWith("Bearer "))
                return null;
            string bearerToken = authorizationHeader.Substring("Bearer ".Length);

            ClaimsPrincipal result = null;
            var tries = 0;

            while (result == null && tries <= 1)
            {
                OpenIdConnectConfiguration config;
                try
                {
                    config = await Configuration.ConfigurationManager.GetConfigurationAsync(CancellationToken.None);
                }
                catch (InvalidOperationException)
                {
                    return null;
                }

                var validationParameter = Configuration.TokenValidationParameterFunc(config, Configuration.Audience);

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
                    Configuration.ConfigurationManager.RequestRefresh();
                    tries++;
                }
                catch (SecurityTokenException)
                {
                    return null;
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }

            return result;
        }
    }
}