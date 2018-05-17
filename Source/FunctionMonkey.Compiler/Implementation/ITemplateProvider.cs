using FunctionMonkey.Model;

namespace FunctionMonkey.Compiler.Implementation
{
    internal interface ITemplateProvider
    {
        string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition);
        string GetJsonTemplate(AbstractFunctionDefinition functionDefinition);
        string GetProxiesJsonTemplate();
    }
}