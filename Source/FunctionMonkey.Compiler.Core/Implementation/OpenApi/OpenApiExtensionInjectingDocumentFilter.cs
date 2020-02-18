using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using YamlDotNet.Serialization;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    class OpenApiExtensionInjectingDocumentFilter : IOpenApiDocumentFilter, IOpenApiExtension
    {
        private readonly OpenApiExtensionSpec _extension;

        public OpenApiExtensionInjectingDocumentFilter(Assembly resourceAssembly, string resourceName)
        {
            var manifestResourceName = $"{resourceAssembly.GetName().Name}.{resourceName}";
            var content = LoadResourceFromAssembly(resourceAssembly, manifestResourceName);
            var deserializer = new DeserializerBuilder().Build();
            _extension = deserializer.Deserialize<OpenApiExtensionSpec>(content);
        }

        public void Apply(OpenApiDocument document, IOpenApiDocumentFilterContext documentFilterContext)
        {
            object instance = document;
            if (!string.IsNullOrWhiteSpace(_extension.path))
            {
                foreach (var openApiPathSegment in _extension.path.Split('.'))
                {
                    var selector = "";
                    var propertyName = openApiPathSegment;
                    if (propertyName.EndsWith("]"))
                    {
                        var index = propertyName.IndexOf("[");
                        selector = propertyName.Substring(index + 1, propertyName.Length - index - 2);
                        propertyName = propertyName.Substring(0, index);
                    }

                    instance = instance.GetType().GetProperty(propertyName).GetValue(instance);
                    if(instance is IList list)
                    {
                        var index = selector.IndexOf("=");
                        if(index == -1)
                        {
                            instance = list[int.Parse(selector)];
                        }
                        else
                        {
                            var selectorValue = selector.Substring(index + 1, selector.Length - index - 1);
                            var selectorName = selector.Substring(0, index);
                            foreach(var item in list)
                            {
                                if(item.GetType().GetProperty(selectorName).GetValue(item).Equals(selectorValue))
                                {
                                    instance = item;
                                    break;
                                }
                            }
                        }
                    }
                    else if (instance is IDictionary dictionary)
                    {
                        instance = dictionary[selector];
                    }
                }
            }
            
            var extensions = instance.GetType().GetProperty("Extensions").GetValue(instance) as IDictionary<string, IOpenApiExtension>;
            extensions.Add(_extension.name, this);
        }

        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            var serializer = new SerializerBuilder().Build();
            writer.WriteRaw(serializer.Serialize(_extension.value));
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
