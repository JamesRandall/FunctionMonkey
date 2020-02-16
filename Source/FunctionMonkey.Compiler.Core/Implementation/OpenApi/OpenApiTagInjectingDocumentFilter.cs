using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi.Models;
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

        public OpenApiTagInjectingDocumentFilter(Assembly resourceAssembly, string resourceName)
        {
            var manifestResourceName = $"{resourceAssembly.GetName().Name}.{resourceName}";
            var content = LoadResourceFromAssembly(resourceAssembly, manifestResourceName);
            var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
            _tag = deserializer.Deserialize<OpenApiTag>(content);
        }

        public void Apply(OpenApiDocument document, IOpenApiDocumentFilterContext documentFilterContext)
        {
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
