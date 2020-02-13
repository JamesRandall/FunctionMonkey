using AzureFromTheTrenches.Commanding.Abstractions;
using System;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Allows for additional HTTP function configuration including Open API and header bindings
    /// </summary>
    public interface IHttpFunctionConfigurationBuilder<TCommand> : IHttpFunctionBuilder,
        IFunctionOptions<
            IHttpFunctionConfigurationBuilder<TCommand>,
            IHttpFunctionOptionsBuilder<TCommand>>
    {
        /// <summary>
        /// The Open API / Swagger description for the endpoint
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> OpenApiDescription(string description);

        /// <summary>
        /// The Open API / Swagger summary for the endpoint
        /// </summary>
        /// <param name="summary">The summary</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> OpenApiSummary(string summary);

        /// <summary>
        /// The Open API / Swagger description for the endpoint and specific response code
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code</param>
        /// <param name="description">The description</param>
        /// <param name="responseType">The type of the response object. Optional</param>
        /// <returns>A IHttpFunctionConfigurationBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionConfigurationBuilder<TCommand> OpenApiResponse(int httpStatusCode, string description, Type responseType = null);
    }
}
