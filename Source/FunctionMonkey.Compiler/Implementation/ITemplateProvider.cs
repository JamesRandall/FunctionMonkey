using FunctionMonkey.Model;

namespace FunctionMonkey.Compiler.Implementation
{
    internal interface ITemplateProvider
    {
        string GetCSharpLinkBackTemplate();
        string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition);
        string GetJsonTemplate(AbstractFunctionDefinition functionDefinition);
        string GetProxiesJsonTemplate();
        string GetTemplate(string name, string type);
    }
}
