namespace FunctionMonkey.Abstractions.Builders
{
    public interface IHttpFunctionOptions<out TParentBuilder> : IFunctionOptions<TParentBuilder> where TParentBuilder : class
    {
        /// <summary>
        /// Allows a token validator to be configured for authorization headers to be
        /// resolved into a ClaimsPrincipal. A validator must be configured for authorization to be applied
        /// to HTTP functions.
        ///
        /// Applying a validator here will override any validator applied in the Authentication block
        /// </summary>
        /// <param name="header">Optional. Allows the token to be picked up from an alternate header rather than
        /// the Authorization header - useful if, for example, you need to support a legacy API Key approach
        /// </param>
        /// <typeparam name="TTokenValidator">The token validators concrete class</typeparam>
        /// <returns>The builder for use in a Fluent API</returns>
        TParentBuilder TokenValidator<TTokenValidator>(string header=null) where TTokenValidator : ITokenValidator;

        TParentBuilder ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>()
            where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization;
    }
}