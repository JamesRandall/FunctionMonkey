using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
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
        /// Add a OpenApiInfo to the generation
        /// </summary>
        /// <param name="name">Name of the document</param>
        /// <param name="openApiInfo">OpenApi document info</param>
        /// <param name="documentRoute">Route to the generated YAML document. Relative to the UserInterfaceRoute</param>
        /// <param name="httpFunctionFilter">Filter the HTTP functions for this OpenApi document</param>
        /// <param name="selected">Set to true, when this document should be selected by default in the drop down box in the UI</param>
        /// <returns></returns>
        IOpenApiBuilder AddOpenApiInfo(string name, string documentRoute, OpenApiInfo openApiInfo, IOpenApiHttpFunctionFilter httpFunctionFilter = null, bool selected = false);

        /// <summary>
        /// Sets the server block in the document
        /// </summary>
        /// <param name="urls">One or more URLs for servers</param>
        IOpenApiBuilder Servers(params string[] urls);

        /// <summary>
        /// If invoked will host a user interface for the Open API spec
        /// </summary>
        IOpenApiBuilder UserInterface(string route = "openapi");

        /// <summary>
        /// If invoked will host a user interface for ReDoc
        /// </summary>
        IOpenApiBuilder ReDocUserInterface(string route = "redoc");

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
        /// Inject the given stylesheet into the index.html
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the stylesheet resource</param>
        /// <param name="media">The media attribute specifies what media/device the target resource is optimized for</param>
        /// <returns></returns>
        IOpenApiBuilder InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen");

        /// <summary>
        /// Inject the given stylesheet into the index.html of ReDoc
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the stylesheet resource</param>
        /// <param name="media">The media attribute specifies what media/device the target resource is optimized for</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen");

        /// <summary>
        /// Inject the given JavaScript into the index.html
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the JavaScript resource</param>
        /// <returns></returns>
        IOpenApiBuilder InjectJavaScript(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given JavaScript into the index.html of ReDoc
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the JavaScript resource</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectJavaScript(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given resource into the index.html
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the resource</param>
        /// <returns></returns>
        IOpenApiBuilder InjectResource(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given resources into the index.html
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourcesDirectoryName">Name of the resources directory</param>
        /// <returns></returns>
        IOpenApiBuilder InjectResources(Assembly resourceAssembly, string resourcesDirectoryName);

        /// <summary>
        /// Inject the given resource into the index.html of ReDoc
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the resource</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectResource(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given resources into the index.html of ReDoc
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourcesDirectoryName">Name of the resources directory</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectResources(Assembly resourceAssembly, string resourcesDirectoryName);

        /// <summary>
        /// Inject the given extension into the OpenApi YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourceName">Name of the extension</param>
        /// <returns></returns>
        IOpenApiBuilder InjectExtension(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given extensions into the OpenApi YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourcesDirectoryName">Name of the extensions directory</param>
        /// <returns></returns>
        IOpenApiBuilder InjectExtensions(Assembly resourceAssembly, string resourcesDirectoryName);

        /// <summary>
        /// Inject the given extension into the ReDoc YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourceName">Name of the extension</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectExtension(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given extensions into the ReDoc YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourcesDirectoryName">Name of the extensions directory</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectExtensions(Assembly resourceAssembly, string resourcesDirectoryName);

        /// <summary>
        /// Inject the given tag into the OpenApi YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourceName">Name of the extension</param>
        /// <returns></returns>
        IOpenApiBuilder InjectTag(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given tags into the OpenApi YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourcesDirectoryName">Name of the extensions directory</param>
        /// <returns></returns>
        IOpenApiBuilder InjectTags(Assembly resourceAssembly, string resourcesDirectoryName);

        /// <summary>
        /// Inject the given tag into the ReDoc YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourceName">Name of the extension</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectTag(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given tag into the ReDoc YAML file
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the extension from</param>
        /// <param name="resourcesDirectoryName">Name of the extensions directory</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectTags(Assembly resourceAssembly, string resourcesDirectoryName);

        /// <summary>
        /// Inject the given logo into the topbar of the index.html
        /// Only one Logo is supported
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the logo</param>
        /// <returns></returns>
        IOpenApiBuilder InjectLogo(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Inject the given logo into the topbar of the index.html of ReDoc
        /// Only one Logo is supported
        /// </summary>
        /// <param name="resourceAssembly">Assembly to load the stylesheet from</param>
        /// <param name="resourceName">Name of the logo</param>
        /// <returns></returns>
        IOpenApiBuilder ReDocInjectLogo(Assembly resourceAssembly, string resourceName);

        /// <summary>
        /// Add a security scheme.
        /// </summary>
        /// <param name="id">The id of this security scheme</param>
        /// <param name="securityScheme">The security scheme to add</param>
        /// <returns></returns>
        IOpenApiBuilder AddSecurityScheme(string id, OpenApiSecurityScheme securityScheme);

        /// <summary>
        /// Add a custom document filter to the filter chain.
        /// </summary>
        /// <param name="documentFilterFactory">The filter to add</param>
        IOpenApiBuilder AddDocumentFilter(Func<IOpenApiDocumentFilter> documentFilterFactory);

        /// <summary>
        /// Add a custom document filter to the filter chain.
        /// </summary>
        /// <param name="documentFilterFactory">The filter to add</param>
        IOpenApiBuilder ReDocAddDocumentFilter(Func<IOpenApiDocumentFilter> documentFilterFactory);

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
