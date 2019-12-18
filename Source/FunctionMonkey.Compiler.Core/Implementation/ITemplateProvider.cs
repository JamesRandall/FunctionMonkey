using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal interface ITemplateProvider
    {
        string GetCSharpLinkBackTemplate();
        string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition);
        string GetJsonTemplate(AbstractFunctionDefinition functionDefinition);
        string GetTemplate(string name, string type);
        string GetCSharpOutputParameterTemplate(AbstractOutputBinding outputBinding);
    }
}
