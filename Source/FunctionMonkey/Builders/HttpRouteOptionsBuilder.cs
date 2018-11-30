using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class HttpRouteOptionsBuilder : IHttpRouteOptionsBuilder
    {
        private readonly HttpRouteConfiguration _httpRouteConfiguration;

        public HttpRouteOptionsBuilder(HttpRouteConfiguration httpRouteConfiguration)
        {
            _httpRouteConfiguration = httpRouteConfiguration;
        }
        
        public IHttpRouteOptionsBuilder ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>() where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            _httpRouteConfiguration.ClaimsPrincipalAuthorizationType = typeof(TClaimsPrincipalAuthorization);
            return this;
        }
    }
}