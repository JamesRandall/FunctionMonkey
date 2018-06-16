namespace FunctionMonkey.Abstractions.Builders
{
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
        /// <param name="title"></param>
        IOpenApiBuilder Title(string title);

        /// <summary>
        /// Sets the server block in the document
        /// </summary>
        /// <param name="urls">One or more URLs for servers</param>
        IOpenApiBuilder Servers(params string[] urls);

        /// <summary>
        /// If invoked will host a user interface for the Open API spec
        /// </summary>
        /// <param name="route">The route to host on - defaults to /swagger</param>
        IOpenApiBuilder UserInterface(string route = "/swagger");
    }
}
