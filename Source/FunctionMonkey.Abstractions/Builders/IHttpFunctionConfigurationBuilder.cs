using System;
using System.Linq.Expressions;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Http.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Allows for additional HTTP function configuration including Open API and header bindings
    /// </summary>
    public interface IHttpFunctionConfigurationBuilder<TCommand> : IHttpFunctionBuilder where TCommand : ICommand
    {
        /// <summary>
        /// The Open API / Swagger description for the endpoint
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> OpenApiDescription(string description);

        /// <summary>
        /// The Open API / Swagger description for the endpoint and specific response code
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code</param>
        /// <param name="description">The description</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> OpenApiResponse(int httpStatusCode, string description);

        /// <summary>
        /// Configure a property on a command to be mapped from a header
        /// </summary>
        /// <param name="property">The property - using the lamnbda format x => x.MyProperty</param>
        /// <param name="headerName">The name of the header to map the property from</param>
        /// <returns></returns>
        IHttpFunctionConfigurationBuilder<TCommand> AddHeaderMapping<TProperty>(
            Expression<Func<TCommand, TProperty>> property, string headerName);

        /// <summary>
        /// Associate a HTTP response handler with this function
        /// </summary>
        /// <typeparam name="TResponseHandler">The type of the handler</typeparam>
        /// <returns></returns>
        IHttpFunctionConfigurationBuilder<TCommand> ResponseHandler<TResponseHandler>()
            where TResponseHandler : IHttpResponseHandler;
    }
}
