using System;
using System.Collections.Generic;
using System.IO;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation
{
    internal class TemplateProvider : ITemplateProvider
    {
        private static readonly Dictionary<Type, string> TypeToTemplatePrefixMap = new Dictionary<Type, string>
        {
            {typeof(HttpFunctionDefinition), "http"},
            {typeof(ServiceBusQueueFunctionDefinition),"servicebusqueue" },
            {typeof(ServiceBusSubscriptionFunctionDefinition),"servicebussubscription" }
        };

        public string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition)
        {
            return GetTemplate(functionDefinition, "csharp");
        }

        public string GetJsonTemplate(AbstractFunctionDefinition functionDefinition)
        {
            return GetTemplate(functionDefinition, "json");
        }

        public string GetProxiesJsonTemplate()
        {
            using (Stream stream = GetType().Assembly.GetManifestResourceStream("FunctionMonkey.Compiler.Templates.proxies.json.handlebars"))
            using (StreamReader reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private string GetTemplate(AbstractFunctionDefinition functionDefinition, string type)
        {
            if (TypeToTemplatePrefixMap.TryGetValue(functionDefinition.GetType(), out string prefix))
            {
                using (Stream stream = GetType().Assembly.GetManifestResourceStream($"FunctionMonkey.Compiler.Templates.{prefix}.{type}.handlebars"))
                using (StreamReader reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
            throw new ConfigurationException($"No templates are configured for function definitions of type {functionDefinition.GetType().Name}");
        }
    }
}
