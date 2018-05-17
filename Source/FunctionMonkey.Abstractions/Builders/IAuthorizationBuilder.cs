using System;

namespace FunctionMonkey.Abstractions.Builders
{
    public enum AuthorizationTypeEnum
    {
        /// <summary>
        /// Anonymous access to functions
        /// </summary>
        Anonymous = 0,
        /// <summary>
        /// Requires a function code
        /// </summary>
        Function = 1,        
        /// <summary>
        /// Requires a valid authorization header token - an ITokenValidator must be configured
        /// </summary>
        TokenValidation = 2,
    };


    public interface IAuthorizationBuilder
    {
        /// <summary>
        /// Allows a token validator to be configured for authorization headers to be
        /// resolved into a ClaimsPrincipal. A validator must be configured for authorization to be applied
        /// to HTTP functions
        /// </summary>
        /// <typeparam name="TTokenValidator">The token validators concrete class</typeparam>
        /// <returns>The builder for use in a Fluent API</returns>
        IAuthorizationBuilder TokenValidator<TTokenValidator>() where TTokenValidator : ITokenValidator;

        /// <summary>
        /// Allows the default authorization mode for HTTP functions to be set. Defaults to anonymous if this method is not used.
        /// </summary>
        /// <param name="authorizationDefault"></param>
        /// <returns>The builder for use in a Fluent API</returns>
        IAuthorizationBuilder AuthorizationDefault(AuthorizationTypeEnum authorizationDefault);

        /// <summary>
        /// Allow claims to be mapped to command properties
        /// </summary>
        /// <param name="claimsMappingBuilder">An action that is given a claims mapping builder</param>
        /// <returns></returns>
        IAuthorizationBuilder Claims(Action<IClaimsMappingBuilder> claimsMappingBuilder);
    }
}
