using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    public class OpenApiTagInjectingDocumentFilter : IOpenApiDocumentFilter
    {
        private readonly OpenApiTag _tag;

        private readonly string _documentRoute;

        public OpenApiTagInjectingDocumentFilter(Assembly resourceAssembly, string resourceName, string documentRoute)
        {
            var manifestResourceName = $"{resourceAssembly.GetName().Name}.{resourceName}";
            var content = LoadResourceFromAssembly(resourceAssembly, manifestResourceName);
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            try
            {
                _tag = deserializer.Deserialize<OpenApiTag>(content);
            }
            catch (Exception e)
            {
                throw new InvalidDataException($"{resourceName}: {e.Message}");
            }

            if(_tag.Description.StartsWith("import:"))
            {
                manifestResourceName = $"{resourceAssembly.GetName().Name}.{_tag.Description.Substring(7).Trim()}";
                content = LoadResourceFromAssembly(resourceAssembly, manifestResourceName);
                _tag.Description = content;
            }

            _documentRoute = documentRoute;
        }

        public void Apply(OpenApiDocument document, IOpenApiDocumentFilterContext documentFilterContext)
        {
            if(!string.IsNullOrWhiteSpace(_documentRoute) && !documentFilterContext.DocumentRoute.StartsWith(_documentRoute))
            {
                return;
            }

            document.Tags.Add(_tag);
        }

        private string LoadResourceFromAssembly(Assembly assembly, string resourceName)
        {
            using (var inputStream = assembly.GetManifestResourceStream(resourceName))
            using (var inputStreamReader = new StreamReader(inputStream, Encoding.UTF8, true))
            {
                return inputStreamReader.ReadToEnd();
            }
        }
    }
}
