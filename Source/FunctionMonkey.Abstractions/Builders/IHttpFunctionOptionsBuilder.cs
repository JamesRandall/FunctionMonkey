using System;
using System.Linq.Expressions;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Http;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IHttpFunctionOptionsBuilder<TCommand>
    {
        /// <summary>
        /// Allows an alternative serializer to be provided. If a naming strategy pair is supplied then this
        /// is ignored.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the serializer</typeparam>
        /// <returns></returns>
        IHttpFunctionOptionsBuilder<TCommand> Serializer<TSerializer>()
            where TSerializer : ISerializer;

        /// <summary>
        /// Allows naming strategies for the default ISerializer implementation
        /// </summary>
        /// <typeparam name="TSerializerNamingStrategy"></typeparam>
        /// <typeparam name="TDeserializerNamingStrategy"></typeparam>
        /// <returns></returns>
        IHttpFunctionOptionsBuilder<TCommand> JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>()
            where TSerializerNamingStrategy : NamingStrategy where TDeserializerNamingStrategy : NamingStrategy;
        
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
        IHttpFunctionOptionsBuilder<TCommand> TokenValidator<TTokenValidator>(string header=null) where TTokenValidator : ITokenValidator;
        
        IHttpFunctionOptionsBuilder<TCommand> ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>()
            where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization;
        
        /// <summary>
        /// Associate a HTTP response handler with this function
        /// </summary>
        /// <typeparam name="TResponseHandler">The type of the handler</typeparam>
        /// <returns></returns>
        IHttpFunctionOptionsBuilder<TCommand> ResponseHandler<TResponseHandler>()
            where TResponseHandler : IHttpResponseHandler;
        
        /// <summary>
        /// Configure a property on a command to be mapped from a header
        /// </summary>
        /// <param name="property">The property - using the lamnbda format x => x.MyProperty</param>
        /// <param name="headerName">The name of the header to map the property from</param>
        /// <returns></returns>
        IHttpFunctionOptionsBuilder<TCommand> AddHeaderMapping<TProperty>(
            Expression<Func<TCommand, TProperty>> property, string headerName);

        /// <summary>
        /// Use this to tell Function Monkey to not attempt to dispatch a command - this can be useful if you
        /// want to, for example, receive a payload on an HTTP API, validate it, then drop it onto a queue
        /// </summary>
        /// <returns></returns>
        IHttpFunctionOptionsBuilder<TCommand> NoCommandHandler();
    }
}