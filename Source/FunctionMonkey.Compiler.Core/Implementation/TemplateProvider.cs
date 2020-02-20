using System;
using System.Collections.Generic;
using System.IO;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using FunctionMonkey.Model.OutputBindings;

namespace FunctionMonkey.Compiler.Core.Implementation
{
    internal class TemplateProvider : ITemplateProvider
    {
        private readonly CompileTargetEnum _target;

        private static readonly Dictionary<Type, string> TypeToTemplatePrefixMap = new Dictionary<Type, string>
        {
            {typeof(HttpFunctionDefinition), "http"},
            {typeof(ServiceBusQueueFunctionDefinition),"servicebusqueue" },
            {typeof(ServiceBusSubscriptionFunctionDefinition),"servicebussubscription" },
            {typeof(StorageQueueFunctionDefinition),"storagequeue" },
            {typeof(BlobFunctionDefinition),"storageblob" },
            {typeof(BlobStreamFunctionDefinition),"storageblobstream" },
            {typeof(TimerFunctionDefinition),"timer" },
            {typeof(CosmosDbFunctionDefinition), "cosmosdb" },
            {typeof(SignalRCommandNegotiateFunctionDefinition), "signalrcommandnegotiate" },
            {typeof(SignalRBindingExpressionNegotiateFunctionDefinition), "signalrbindingexpressionnegotiate" },
            {typeof(SignalRClaimNegotiateFunctionDefinition), "signalrclaimnegotiate" },
            {typeof(EventHubFunctionDefinition),"eventhub" },
            // output bindings
            {typeof(ServiceBusQueueOutputBinding), "servicebusqueue" },
            {typeof(ServiceBusTopicOutputBinding), "servicebustopic" },
            {typeof(CosmosOutputBinding), "cosmosdb" },
            {typeof(StorageBlobOutputBinding), "storageblob"},
            {typeof(StorageQueueOutputBinding), "storagequeue"},
            {typeof(StorageTableOutputBinding), "storagetable"},
            {typeof(SignalROutputBinding), "signalr" },
            {typeof(EventHubOutputBinding), "eventhub"}
        };

        public TemplateProvider(CompileTargetEnum target)
        {
            _target = target;
        }

        public string GetCSharpTemplate(AbstractFunctionDefinition functionDefinition)
        {
            return GetTemplate(functionDefinition, "csharp");
        }

        public string GetTemplate(string name, string type)
        {
            string targetPrefix = GetTargetPrefix();
            using (Stream stream = GetType().Assembly
                .GetManifestResourceStream($"FunctionMonkey.Compiler.Core.Templates.{targetPrefix}.{name}.{type}.handlebars"))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                        return reader.ReadToEnd();
                }
            }

            throw new ConfigurationException($"No templates are configured for function definitions of type {name}");
        }

        public string GetCSharpOutputParameterTemplate(AbstractOutputBinding outputBinding)
        {
            return GetTemplate(outputBinding, "outputparameter.csharp");
        }

        public string GetJsonOutputParameterTemplate(AbstractOutputBinding outputBinding)
        {
            try
            {
                return GetTemplate(outputBinding, "output.json");
            }
            catch (ConfigurationException)
            {
                return null;
            }            
        }

        public string GetCSharpOutputCollectorTemplate(AbstractOutputBinding outputBinding)
        {
            const string prefix = "outputcollector.csharp";

            return GetTemplate(outputBinding, prefix, true);            
            
        }

        public string GetJsonTemplate(AbstractFunctionDefinition functionDefinition)
        {
            return GetTemplate(functionDefinition, "json");
        }

        private string GetTemplate(object functionDefinition, string type, bool fallbackToDefault=false)
        {
            string template = null;
            if (TypeToTemplatePrefixMap.TryGetValue(functionDefinition.GetType(), out string prefix))
            {
                string targetPrefix = GetTargetPrefix();
                using (Stream stream = GetType().Assembly
                    .GetManifestResourceStream($"FunctionMonkey.Compiler.Core.Templates.{targetPrefix}.{prefix}.{type}.handlebars"))
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                            template = reader.ReadToEnd();
                    }                                        
                }

                if (string.IsNullOrWhiteSpace(template) && fallbackToDefault)
                {
                    using (Stream stream = GetType().Assembly
                        .GetManifestResourceStream($"FunctionMonkey.Compiler.Core.Templates.{targetPrefix}.default.{type}.handlebars"))
                    {
                        if (stream != null)
                        {
                            using (StreamReader reader = new StreamReader(stream))
                                template = reader.ReadToEnd();
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(template))
                {
                    return template;
                }
            }
            throw new ConfigurationException($"No templates are configured for function definitions of type {functionDefinition.GetType().Name}");
        }

        private string GetTargetPrefix()
        {
            return _target == CompileTargetEnum.AzureFunctions ? "AzureFunctions" : "AspNetCore";
        }

        public string GetCSharpLinkBackTemplate()
        {
            string targetPrefix = GetTargetPrefix();
            using (Stream stream = GetType().Assembly.GetManifestResourceStream(
                $"FunctionMonkey.Compiler.Core.Templates.{targetPrefix}.forcereference.csharp.handlebars"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            
        }
    }
}
