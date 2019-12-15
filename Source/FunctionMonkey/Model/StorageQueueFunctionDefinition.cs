using System;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model
{
    public class StorageQueueFunctionDefinition : AbstractFunctionDefinition
    {
        public StorageQueueFunctionDefinition(Type commandType) : base("StqFn", commandType)
        {
        }
        
        public StorageQueueFunctionDefinition(Type commandType, Type resultType) : base("StqFn", commandType, resultType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string QueueName { get; set; }
    }
}
