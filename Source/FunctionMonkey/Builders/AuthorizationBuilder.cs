using System;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
{
    internal class AuthorizationBuilder : IAuthorizationBuilder
    {
        public ClaimsMappingBuilder ClaimsMappingBuilder { get; } = new ClaimsMappingBuilder();

        public Type TokenValidatorType { get; set; }

        public AuthorizationTypeEnum AuthorizationDefaultValue { get; set; } = AuthorizationTypeEnum.Anonymous;

        public IAuthorizationBuilder TokenValidator<TTokenValidator>() where TTokenValidator : ITokenValidator
        {
            TokenValidatorType = typeof(TTokenValidator);
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
