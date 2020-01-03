using System.Collections.Generic;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    internal class OpenApiFileReference
    {
        public string Filename { get; set; }

        public string Content { get; set; }
    }

    internal class OpenApiOutputModel
    {
        public OpenApiFileReference OpenApiSpecification { get; set; }

        public IReadOnlyCollection<OpenApiFileReference> SwaggerUserInterface { get; set; }

        public bool IsConfiguredForUserInterface => SwaggerUserInterface != null && SwaggerUserInterface.Count > 0;
    }
}
