using System;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model
{
    public class ServiceBusQueueFunctionDefinition : AbstractFunctionDefinition
    {
        public ServiceBusQueueFunctionDefinition(Type commandType) : base("SbqFn",commandType)
        {
        }
        
        public ServiceBusQueueFunctionDefinition(Type commandType, Type explicitCommandResultType) : base("SbqFn", commandType, explicitCommandResultType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string QueueName { get; set; }
        
        public bool IsSessionEnabled { get; set; }
    }
}
