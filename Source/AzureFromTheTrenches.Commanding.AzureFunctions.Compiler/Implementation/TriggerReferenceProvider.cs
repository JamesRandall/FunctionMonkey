using System;
using System.Collections.Generic;
using System.Reflection;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;
using Microsoft.Azure.WebJobs;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Compiler.Implementation
{
    internal interface ITriggerReferenceProvider
    {
        Assembly GetTriggerReference(AbstractFunctionDefinition functionDefinition);
    }

    internal class TriggerReferenceProvider : ITriggerReferenceProvider
    {
        private static readonly Dictionary<Type, Assembly> TriggerReferences = new Dictionary<Type, Assembly>
        {
            {typeof(HttpFunctionDefinition), typeof(HttpTriggerAttribute).Assembly},
            {typeof(ServiceBusQueueFunctionDefinition), typeof(ServiceBusTriggerAttribute).Assembly },
            {typeof(ServiceBusSubscriptionFunctionDefinition), typeof(ServiceBusTriggerAttribute).Assembly }
        };

        public Assembly GetTriggerReference(AbstractFunctionDefinition functionDefinition)
        {
            if (TriggerReferences.TryGetValue(functionDefinition.GetType(), out Assembly assembly))
            {
                return assembly;
            }
            throw new ConfigurationException($"No trigger reference mapping configured for a function of type {functionDefinition.GetType().Name}");
        }
    }
}
