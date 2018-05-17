using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation
{
    internal interface ITemplateProvider
    {
        string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition);
        string GetJsonTemplate(AbstractFunctionDefinition functionDefinition);
        string GetProxiesJsonTemplate();
    }
}