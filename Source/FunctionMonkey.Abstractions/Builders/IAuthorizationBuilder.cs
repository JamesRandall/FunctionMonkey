using System;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Abstraction over Azure Functions authorization types with the addition of the token validation model provided
    /// by this framework
    /// </summary>
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

    /// <summary>
    /// Provides an interface that allows authorization to be configured
    /// </summary>
    public interface IAuthorizationBuilder
    {
        /// <summary>
        /// Allows a token validator to be configured for authorization headers to be
        /// resolved into a ClaimsPrincipal. A validator must be configured for authorization to be applied
        /// to HTTP functions
        /// </summary>
        /// <param name="header">Optional. Allows the token to be picked up from an alternate header rather than
        /// the Authorization header - useful if, for example, you need to support a legacy API Key approach
        /// </param>
        /// <typeparam name="TTokenValidator">The token validators concrete class</typeparam>
        /// <returns>The builder for use in a Fluent API</returns>
        IAuthorizationBuilder TokenValidator<TTokenValidator>(string header=null) where TTokenValidator : ITokenValidator;

        /// <summary>
        /// Allows a custom claims binder to be registered. This can only occur globally and must take responsibility
        /// for all claims mapping.
        /// </summary>
        /// <typeparam name="TCustomClaimsBinder">The type of the claims binder</typeparam>
        /// <returns>The builder for use in a Fluent API</returns>
        IAuthorizationBuilder CustomClaimsBinder<TCustomClaimsBinder>()
            where TCustomClaimsBinder : ICommandClaimsBinder;

        /// <summary>
        /// Allows the default authorization mode for HTTP functions to be set. Defaults to Function Code if this method is not used.
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

        /// <summary>
        /// Set a default claims principal authorization type for all HTTP routes
        /// </summary>
        /// <typeparam name="TAuthorizationType">The authorizer - must implement IClaimsPrincipalAuthorization</typeparam>
        /// <returns></returns>
        IAuthorizationBuilder ClaimsPrincipalAuthorizationDefault<TAuthorizationType>()
            where TAuthorizationType : IClaimsPrincipalAuthorization;
    }
}
