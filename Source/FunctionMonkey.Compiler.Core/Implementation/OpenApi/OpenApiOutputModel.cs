using System.Collections.Generic;

namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    internal class OpenApiFileReference
    {
        public string Filename { get; set; }

        public byte[] Content { get; set; }
    }

    internal class OpenApiOutputModel
    {
        public IList<OpenApiFileReference> OpenApiFileReferences { get; } = new List<OpenApiFileReference>();

        public bool IsConfiguredForUserInterface => OpenApiFileReferences.Count > 0;

        public string UserInterfaceRoute { get; set; }

        public string RedocUserInterfaceRoute { get; set; }
    }
}
