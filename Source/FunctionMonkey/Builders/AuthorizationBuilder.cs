using System;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Extensions;

namespace FunctionMonkey.Builders
{
    internal class AuthorizationBuilder : IAuthorizationBuilder
    {
        public ClaimsMappingBuilder ClaimsMappingBuilder { get; } = new ClaimsMappingBuilder();

        public Type TokenValidatorType { get; set; }

        public string AuthorizationHeader { get; set; }
        
        public string WellKnownEndpoint { get; set; }

        public Type DefaultClaimsPrincipalAuthorizationType { get; set; }
        
        public Type CustomClaimsBinderType { get; set; }

        public AuthorizationTypeEnum AuthorizationDefaultValue { get; set; } = AuthorizationTypeEnum.Function;

        public IAuthorizationBuilder TokenValidator<TTokenValidator>(string header=null) where TTokenValidator : ITokenValidator
        {
            TokenValidatorType = typeof(TTokenValidator);
            AuthorizationHeader = header;
            return this;
        }

        public IAuthorizationBuilder CustomClaimsBinder<TCustomClaimsBinder>() where TCustomClaimsBinder : ICommandClaimsBinder
        {
            CustomClaimsBinderType = typeof(TCustomClaimsBinder);
            return this;
        }

        public IAuthorizationBuilder AuthorizationDefault(AuthorizationTypeEnum authorizationDefault)
        {
            AuthorizationDefaultValue = authorizationDefault;
            return this;
        }

        public IAuthorizationBuilder Claims(Action<IClaimsMappingBuilder> mapping)
        {
            mapping(ClaimsMappingBuilder);
            return this;
        }

        public IAuthorizationBuilder ClaimsPrincipalAuthorizationDefault<TAuthorizationType>() where TAuthorizationType : IClaimsPrincipalAuthorization
        {
            DefaultClaimsPrincipalAuthorizationType = typeof(TAuthorizationType);
            return this;
        }
    }
}
