using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;

namespace FunctionMonkey.Compiler.Implementation
{
    internal interface ITemplateProvider
    {
        string GetCSharpLinkBackTemplate();
        string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition);
        string GetJsonTemplate(AbstractFunctionDefinition functionDefinition);
        string GetTemplate(string name, string type);
        string GetCSharpOutputTemplate(AbstractOutputBinding outputBinding);
    }
}
