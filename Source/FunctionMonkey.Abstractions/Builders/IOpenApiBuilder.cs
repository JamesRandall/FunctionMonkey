namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// The Api specification used for generating the definition file(s).
    /// </summary>
    public enum ApiSpecVersion
    {
        /// <summary>
        /// Represents OpenAPI V2.0 spec
        /// </summary>
        OpenApi2_0 = 0,

        /// <summary>
        /// Represents all patches of OpenAPI V3.0 spec (e.g. 3.0.0, 3.0.1)
        /// </summary>  
        OpenApi3_0 = 1
    };

    /// <summary>
    /// The output format used for the generated definition file(s).
    /// </summary>
    public enum ApiOutputFormat
    {
        /// <summary>
        /// Yaml format.
        /// </summary>
        Yaml = 0,

        /// <summary>
        /// JSON format.
        /// </summary>
        Json = 1,
    };

    /// <summary>
    /// An interface that allows an Open API document to be built.
    /// </summary>
    public interface IOpenApiBuilder
    {
        /// <summary>
        /// Sets the version for the document
        /// </summary>
        /// <param name="version">Document version</param>
        IOpenApiBuilder Version(string version);

        /// <summary>
        /// Sets the title of the Open API document
        /// </summary>
        /// <param name="title">Document title</param>
        IOpenApiBuilder Title(string title);

        /// <summary>
        /// Sets the server block in the document
        /// </summary>
        /// <param name="urls">One or more URLs for servers</param>
        IOpenApiBuilder Servers(params string[] urls);

        /// <summary>
        /// Sets the open API specification version for the generated document
        /// </summary>
        /// <param name="specVersion">Specification version</param>
        IOpenApiBuilder ApiSpecVersion(ApiSpecVersion specVersion);

        /// <summary>
        /// Sets the open API specification output format for the generated document
        /// </summary>
        /// <param name="outputFormat">Output format</param>
        IOpenApiBuilder ApiOutputFormat(ApiOutputFormat outputFormat);
        
        /// <summary>
        /// If invoked will host a user interface for the Open API spec
        /// </summary>
        /// <param name="route">The route to host on - defaults to /swagger</param>
        IOpenApiBuilder UserInterface(string route = "/openapi");
    }
}
