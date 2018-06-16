using System;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;

namespace FunctionMonkey.Builders
{
    internal class AuthorizationBuilder : IAuthorizationBuilder
    {
        public ClaimsMappingBuilder ClaimsMappingBuilder { get; } = new ClaimsMappingBuilder();

        public Type TokenValidatorType { get; set; }

        public string AuthorizationHeader { get; set; }

        public AuthorizationTypeEnum AuthorizationDefaultValue { get; set; } = AuthorizationTypeEnum.Function;

        public IAuthorizationBuilder TokenValidator<TTokenValidator>(string header=null) where TTokenValidator : ITokenValidator
        {
            TokenValidatorType = typeof(TTokenValidator);
            AuthorizationHeader = header;
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
    }
}
