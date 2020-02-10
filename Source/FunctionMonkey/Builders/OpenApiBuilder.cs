using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Model;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
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

        public IOpenApiBuilder AddOpenApiInfo(string name, string documentRoute, OpenApiInfo openApiInfo, IOpenApiHttpFunctionFilter httpFunctionFilter = null, bool selected = false)
        {
            _openApiConfiguration.OpenApiDocumentInfos.Add(name, new OpenApiDocumentInfo
            {
                DocumentRoute = documentRoute,
                OpenApiInfo = openApiInfo,
                HttpFunctionFilter = httpFunctionFilter,
                Selected = selected
            });
            return this;
        }

        public IOpenApiBuilder Servers(params string[] urls)
        {
            _openApiConfiguration.Servers = urls;
            return this;
        }

        public IOpenApiBuilder UserInterface(string route = "openapi")
        {
            if(route.StartsWith("/"))
            {
                route = route.Substring(1);
            }
            _openApiConfiguration.UserInterfaceRoute = route;
            return this;
        }

        public IOpenApiBuilder RedocUserInterface(string route = "redoc")
        {
            if (route.StartsWith("/"))
            {
                route = route.Substring(1);
            }
            _openApiConfiguration.RedocUserInterfaceRoute = route;
            return this;
        }

        public IOpenApiBuilder AddXmlComments(Func<XPathDocument> xmlDocFactory)
        {
            _openApiConfiguration.XmlDocFactories.Add(xmlDocFactory);
            return this;
        }

        public IOpenApiBuilder InjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen")
        {
            _openApiConfiguration.InjectedStylesheets.Add((resourceAssembly, resourceName, media));
            return this;
        }

        public IOpenApiBuilder RedocInjectStylesheet(Assembly resourceAssembly, string resourceName, string media = "screen")
        {
            _openApiConfiguration.RedocInjectedStylesheets.Add((resourceAssembly, resourceName, media));
            return this;
        }

        public IOpenApiBuilder InjectJavaScript(Assembly resourceAssembly, string resourceName)
        {
            _openApiConfiguration.InjectedJavaScripts.Add((resourceAssembly, resourceName));
            return this;
        }

        public IOpenApiBuilder RedocInjectJavaScript(Assembly resourceAssembly, string resourceName)
        {
            _openApiConfiguration.RedocInjectedJavaScripts.Add((resourceAssembly, resourceName));
            return this;
        }

        public IOpenApiBuilder InjectResource(Assembly resourceAssembly, string resourceName)
        {
            _openApiConfiguration.InjectedResources.Add((resourceAssembly, resourceName));
            return this;
        }

        public IOpenApiBuilder RedocInjectResource(Assembly resourceAssembly, string resourceName)
        {
            _openApiConfiguration.RedocInjectedResources.Add((resourceAssembly, resourceName));
            return this;
        }

        public IOpenApiBuilder InjectLogo(Assembly resourceAssembly, string resourceName)
        {
            _openApiConfiguration.InjectedLogo = (resourceAssembly, resourceName);
            return this;
        }

        public IOpenApiBuilder RedocInjectLogo(Assembly resourceAssembly, string resourceName)
        {
            _openApiConfiguration.RedocInjectedLogo = (resourceAssembly, resourceName);
            return this;
        }

        public IOpenApiBuilder AddXmlComments(string filePath)
        {
            _openApiConfiguration.XmlDocFactories.Add(() => new XPathDocument(filePath));
            return this;
        }

        public IOpenApiBuilder AddSecurityScheme(string id, OpenApiSecurityScheme securityScheme)
        {
            _openApiConfiguration.SecuritySchemes.Add(id, securityScheme);
            return this;
        }

        public IOpenApiBuilder AddDocumentFilter(Func<IOpenApiDocumentFilter> documentFilterFactory)
        {
            _openApiConfiguration.DocumentFilterFactories.Add(documentFilterFactory);
            return this;
        }

        public IOpenApiBuilder RedocAddDocumentFilter(Func<IOpenApiDocumentFilter> documentFilterFactory)
        {
            _openApiConfiguration.RedocDocumentFilterFactories.Add(documentFilterFactory);
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
