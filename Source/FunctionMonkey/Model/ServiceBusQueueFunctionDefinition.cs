using System;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model
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
