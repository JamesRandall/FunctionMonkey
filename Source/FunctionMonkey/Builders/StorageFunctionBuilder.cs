using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class StorageFunctionBuilder : IStorageFunctionBuilder
    {
        private readonly string _connectionName;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public StorageFunctionBuilder(string connectionName, List<AbstractFunctionDefinition> definitions)
        {
            _connectionName = connectionName;
            _definitions = definitions;
        }

        public IStorageFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand
        {
            _definitions.Add(new StorageQueueFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                QueueName = queueName
            });
            return this;
        }
    }
}
