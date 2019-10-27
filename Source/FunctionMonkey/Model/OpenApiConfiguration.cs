using FunctionMonkey.Abstractions.OpenApi;
using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace FunctionMonkey.Model
{
    public class OpenApiConfiguration
    {
        public string Version { get; set; }

        public string Title { get; set; }

        public IReadOnlyCollection<string> Servers { get; set; }

        public bool IsOpenApiOutputEnabled => !string.IsNullOrWhiteSpace(Version) && !string.IsNullOrWhiteSpace(Title);

        public bool IsValid
        {
            get
            {
                int requiredSettingCount = 0;
                if (!string.IsNullOrWhiteSpace(Version)) requiredSettingCount++;
                if (!string.IsNullOrWhiteSpace(Title)) requiredSettingCount++;
                return requiredSettingCount == 0 || requiredSettingCount == 2;
            }
        }

        public string UserInterfaceRoute { get; set; }

        public Func<XPathDocument> IncludeXmlComments { get; set; }

        public IList<Func<IOpenApiDocumentFilter>> DocumentFilterFactories { get; } = new List<Func<IOpenApiDocumentFilter>>();

        public IList<Func<IOpenApiOperationFilter>> OperationFilterFactories { get; } = new List<Func<IOpenApiOperationFilter>>();

        public IList<Func<IOpenApiParameterFilter>> ParameterFilterFactories { get; } = new List<Func<IOpenApiParameterFilter>>();

        public IList<Func<IOpenApiSchemaFilter>> SchemaFilterFactories { get; } = new List<Func<IOpenApiSchemaFilter>>();
    }
}
