namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Used to optionally annotate a function with metadata for Open API
    /// </summary>
    public interface IHttpFunctionBuilderMetadataBuilder : IHttpFunctionBuilder
    {
        IHttpFunctionBuilderMetadataBuilder Description(string description);

        IHttpFunctionBuilderMetadataBuilder Response(int httpStatusCode, string description);
    }
}
