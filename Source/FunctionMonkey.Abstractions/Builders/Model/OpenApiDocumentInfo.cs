using Microsoft.OpenApi.Models;

namespace FunctionMonkey.Abstractions.Builders.Model
{
    public class OpenApiDocumentInfo
    {
        public OpenApiInfo OpenApiInfo { get; set; }

        public string DocumentRoute { get; set; }

        public bool Selected { get; set; }
    }
}
