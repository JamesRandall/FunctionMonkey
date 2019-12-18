using System;
using System.Xml.XPath;

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
        IOpenApiBuilder UserInterface(string route = "/openapi");

        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files
        /// </summary>
        /// <param name="xmlDocFactory">A factory method that returns XML Comments as an XPathDocument</param>
        IOpenApiBuilder AddXmlComments(Func<XPathDocument> xmlDocFactory);

        /// <summary>
        /// Inject human-friendly descriptions for Operations, Parameters and Schemas based on XML Comment files
        /// </summary>
        /// <param name="filePath">An abolsute path to the file that contains XML Comments</param>
        IOpenApiBuilder AddXmlComments(string filePath);

        /// <summary>
        /// Add a custom document filter to the filter chain.
        /// </summary>
        /// <param name="documentFilterFactory">The filter to add</param>
        IOpenApiBuilder AddDocumentFilter(Func<IOpenApiDocumentFilter> documentFilterFactory);

        /// <summary>
        /// Add a custom operation filter to the filter chain.
        /// </summary>
        /// <param name="operationFilterFactory">The filter to add</param>
        IOpenApiBuilder AddOperationFilter(Func<IOpenApiOperationFilter> operationFilterFactory);

        /// <summary>
        /// Add a custom parameter filter to the filter chain.
        /// </summary>
        /// <param name="parameterFilterFactory">The filter to add</param>
        IOpenApiBuilder AddParameterFilter(Func<IOpenApiParameterFilter> parameterFilterFactory);

        /// <summary>
        /// Add a custom schema filter to the filter chain.
        /// </summary>
        /// <param name="schemaFilterFactory">The filter to add</param>
        IOpenApiBuilder AddSchemaFilter(Func<IOpenApiSchemaFilter> schemaFilterFactory);

        /// <summary>
        /// Provide a custom strategy for generating the unique Id's that are used to reference object Schemas.
        /// </summary>
        /// <param name="schemaIdSelector">
        /// A lambda that returns a unique identifier for the provided system type
        /// </param>
        IOpenApiBuilder CustomSchemaIds(Func<Type, string> schemaIdSelector);
    }
}
