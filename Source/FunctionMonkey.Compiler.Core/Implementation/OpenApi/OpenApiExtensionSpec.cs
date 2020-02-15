namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    public class OpenApiExtensionSpec
    {
        public string openApiPath;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "YAML")]
        public string propertyName { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "YAML")]
        public string propertyValue { get; set; }
    }
}
