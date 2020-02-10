using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.XPath;

namespace FunctionMonkey.Abstractions.Http
{
    public class OpenApiConfiguration
    {
        public string Title
        {
            set
            {
                if (OpenApiDocumentInfos.TryGetValue("default", out var openApiDocumentInfo))
                {
                    openApiDocumentInfo.OpenApiInfo.Title = value;
                }
                else
                {
                    OpenApiDocumentInfos.Add("default", new OpenApiDocumentInfo
                    {
                        DocumentRoute = "openapi.yaml",
                        OpenApiInfo = new OpenApiInfo
                        {
                            Title = value
                        }
                    });
                }
            }
        }

        public string Version
        {
            set
            {
                if (OpenApiDocumentInfos.TryGetValue("default", out var openApiDocumentInfo))
                {
                    openApiDocumentInfo.OpenApiInfo.Version = value;
                }
                else
                {
                    OpenApiDocumentInfos.Add("default", new OpenApiDocumentInfo 
                    {
                        DocumentRoute = "openapi.yaml",
                        OpenApiInfo = new OpenApiInfo
                        {
                            Version = value
                        }
                    });
                }
            }
        }

        public IDictionary<string, OpenApiDocumentInfo> OpenApiDocumentInfos = new Dictionary<string, OpenApiDocumentInfo>();

        public IReadOnlyCollection<string> Servers { get; set; }

        public bool IsOpenApiOutputEnabled => OpenApiDocumentInfos.Count != 0;

        public bool IsValid
        {
            get
            {
                foreach(var keyValuePair in OpenApiDocumentInfos)
                {
                    if(string.IsNullOrWhiteSpace(keyValuePair.Value.OpenApiInfo.Version) || string.IsNullOrWhiteSpace(keyValuePair.Value.OpenApiInfo.Title))
                    {
                        return false;
                    }
                }                
                return true;
            }
        }

        public string UserInterfaceRoute { get; set; }
        
        public IList<Func<XPathDocument>> XmlDocFactories { get; } = new List<Func<XPathDocument>>();

        public IList<(Assembly resourceAssembly, string resourceName, string media)> InjectedStylesheets { get; } = new List<(Assembly resourceAssembly, string resourceName, string media)>();

        public IList<(Assembly resourceAssembly, string resourceName)> InjectedResources { get; } = new List<(Assembly resourceAssembly, string resourceName)>();

        public (Assembly resourceAssembly, string resourceName) InjectedLogo { get; set; } 

        public IList<(Assembly resourceAssembly, string resourceName)> InjectedJavaScripts { get; } = new List<(Assembly resourceAssembly, string resourceName)>();

        public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();

        public IList<Func<IOpenApiDocumentFilter>> DocumentFilterFactories { get; } = new List<Func<IOpenApiDocumentFilter>>();

        public IList<Func<IOpenApiOperationFilter>> OperationFilterFactories { get; } = new List<Func<IOpenApiOperationFilter>>();

        public IList<Func<IOpenApiParameterFilter>> ParameterFilterFactories { get; } = new List<Func<IOpenApiParameterFilter>>();

        public IList<Func<IOpenApiSchemaFilter>> SchemaFilterFactories { get; } = new List<Func<IOpenApiSchemaFilter>>();

        public Func<Type, string> SchemaIdSelector { get; set; }
    }
}
