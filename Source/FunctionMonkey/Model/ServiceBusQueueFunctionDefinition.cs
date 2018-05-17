using System;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Model
{
    public class ServiceBusQueueFunctionDefinition : AbstractFunctionDefinition
    {
        public ServiceBusQueueFunctionDefinition(Type commandType) : base("SbqFn",commandType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string QueueName { get; set; }
    }
}
