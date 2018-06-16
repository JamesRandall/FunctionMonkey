namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Used to optionally annotate a function with metadata for Open API
    /// </summary>
    public interface IHttpFunctionBuilderMetadataBuilder : IHttpFunctionBuilder
    {
        /// <summary>
        /// The Open API / Swagger description for the endpoint
        /// </summary>
        /// <param name="description">The description</param>
        /// <returns>A IHttpFunctionBuilderMetadataBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionBuilderMetadataBuilder Description(string description);

        /// <summary>
        /// The Open API / Swagger description for the endpoint and specific response code
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code</param>
        /// <param name="description">The description</param>
        /// <returns>A IHttpFunctionBuilderMetadataBuilder that allows further HTTP functions to be created and this function to be further configured with Open API / Swagger metadata.</returns>
        IHttpFunctionBuilderMetadataBuilder Response(int httpStatusCode, string description);
    }
}
