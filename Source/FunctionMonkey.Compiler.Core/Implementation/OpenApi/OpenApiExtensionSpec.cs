namespace FunctionMonkey.Compiler.Core.Implementation.OpenApi
{
    public class OpenApiExtensionSpec
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "YAML")]
        public string path;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "YAML")]
        public string name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "YAML")]
        public object value { get; set; }
    }
}
