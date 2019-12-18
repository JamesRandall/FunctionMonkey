using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Model;
using System;
using System.Xml.XPath;

namespace FunctionMonkey.Builders
{
    internal class OpenApiBuilder : IOpenApiBuilder
    {
        private readonly OpenApiConfiguration _openApiConfiguration;

        public OpenApiBuilder(OpenApiConfiguration openApiConfiguration)
        {
            _openApiConfiguration = openApiConfiguration;
        }

        public IOpenApiBuilder Version(string version)
        {
            _openApiConfiguration.Version = version;
            return this;
        }

        public IOpenApiBuilder Title(string title)
        {
            _openApiConfiguration.Title = title;
            return this;
        }

        public IOpenApiBuilder Servers(params string[] urls)
        {
            _openApiConfiguration.Servers = urls;
            return this;
        }

        public IOpenApiBuilder UserInterface(string route = "/swagger")
        {
            _openApiConfiguration.UserInterfaceRoute = route;
            return this;
        }

        public IOpenApiBuilder AddXmlComments(Func<XPathDocument> xmlDocFactory)
        {
            _openApiConfiguration.XmlDocFactories.Add(xmlDocFactory);
            return this;
        }

        public IOpenApiBuilder AddXmlComments(string filePath)
        {
            _openApiConfiguration.XmlDocFactories.Add(() => new XPathDocument(filePath));
            return this;
        }

        public IOpenApiBuilder AddDocumentFilter(Func<IOpenApiDocumentFilter> documentFilterFactory)
        {
            _openApiConfiguration.DocumentFilterFactories.Add(documentFilterFactory);
            return this;
        }

        public IOpenApiBuilder AddOperationFilter(Func<IOpenApiOperationFilter> operationFilterFactory)
        {
            _openApiConfiguration.OperationFilterFactories.Add(operationFilterFactory);
            return this;
        }

        public IOpenApiBuilder AddParameterFilter(Func<IOpenApiParameterFilter> parameterFilterFactory)
        {
            _openApiConfiguration.ParameterFilterFactories.Add(parameterFilterFactory);
            return this;
        }

        public IOpenApiBuilder AddSchemaFilter(Func<IOpenApiSchemaFilter> schemaFilterFactory)
        {
            _openApiConfiguration.SchemaFilterFactories.Add(schemaFilterFactory);
            return this;
        }

        public IOpenApiBuilder CustomSchemaIds(Func<Type, string> schemaIdSelector)
        {
            _openApiConfiguration.SchemaIdSelector = schemaIdSelector;
            return this;
        }
    }
}
