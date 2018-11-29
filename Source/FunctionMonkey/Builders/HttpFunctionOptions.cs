using FunctionMonkey.Abstractions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class HttpFunctionOptions : FunctionOptions
    {
        private readonly HttpFunctionDefinition _functionDefinition;

        public HttpFunctionOptions(HttpFunctionDefinition functionDefinition) : base(functionDefinition)
        {
            _functionDefinition = functionDefinition;
        }
        
        public void TokenValidator<TTokenValidator>(string header=null) where TTokenValidator : ITokenValidator
        {
            _functionDefinition.TokenValidatorType = typeof(TTokenValidator);
            if (header != null)
            {
                _functionDefinition.TokenHeader = header;
            }
        }
        
        
        public void ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>()
            where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            _functionDefinition.ClaimsPrincipalAuthorizationType = typeof(TClaimsPrincipalAuthorization);
        }
    }
}