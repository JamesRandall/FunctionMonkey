using System;
using System.Collections.Generic;
using System.Text;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace OpenApi
{
    public class CustomReDocDocumentFilter : IOpenApiDocumentFilter
    {
        public void Apply(OpenApiDocument document, IOpenApiDocumentFilterContext documentFilterContext)
        {
            document.Extensions.Add("x-tagGroups", new FooExtension());
            
            document.Tags.Add(new OpenApiTag
            {
                Name = "Introduction",
                Description = "Description text"
            });
        }
    }

    internal class FooExtension : IOpenApiExtension
    {
        public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion)
        {
            writer.WriteRaw("[    {      \"name\": \"Quick Start\",      \"tags\": [\"Introduction\",\"FooBar\", \"ImportMarkup\"]    },    {      \"name\": \"API\",      \"tags\": [\"Customers\"]    }  ]");

        }
    }
}
