using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Commanding.Abstractions;
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

        /// <summary>
        /// Creates a function for a storage queue
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>Builder for use in a fluent API</returns>
        public IStorageFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand
        {
            _definitions.Add(new StorageQueueFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                QueueName = queueName
            });
            return this;
        }

        /// <summary>
        /// Creates a function for a blob trigger
        /// </summary>
        /// <typeparam name="TCommand">The type of the command. If the command implements IStreamCommand then
        /// it will be passed the blob as an open stream - otherwise the blob will be deserialized into
        /// the command</typeparam>
        /// <param name="blobPath">Container and optional pattern for the blob</param>
        /// <returns>Builder for use in a fluent API</returns>
        public IStorageFunctionBuilder BlobFunction<TCommand>(string blobPath) where TCommand : ICommand
        {
            if (typeof(IStreamCommand).IsAssignableFrom(typeof(TCommand)))
            {
                _definitions.Add(new BlobStreamFunctionDefinition(typeof(TCommand))
                {
                    ConnectionStringName = _connectionName,
                    BlobPath = blobPath
                });
            }
            else
            {
                _definitions.Add(new BlobFunctionDefinition(typeof(TCommand))
                {
                    ConnectionStringName = _connectionName,
                    BlobPath = blobPath
                });
            }
            

            return this;
        }
    }
}
