using System;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model
{
    public class StorageQueueFunctionDefinition : AbstractFunctionDefinition
    {
        public StorageQueueFunctionDefinition(Type commandType) : base("StqFn", commandType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string QueueName { get; set; }
    }
}
