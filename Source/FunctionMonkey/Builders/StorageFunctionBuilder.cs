using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class StorageFunctionBuilder : IStorageFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly string _connectionName;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public StorageFunctionBuilder(ConnectionStringSettingNames connectionStringSettingNames, string connectionName, List<AbstractFunctionDefinition> definitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _connectionName = connectionName;
            _definitions = definitions;
        }

        /// <summary>
        /// Creates a function for a storage queue
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>Builder for use in a fluent API</returns>
        public IStorageFunctionOptionBuilder<TCommand> QueueFunction<TCommand>(string queueName)
        {
            StorageQueueFunctionDefinition definition = new StorageQueueFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                QueueName = queueName
            };
            _definitions.Add(definition);
            return new StorageFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }

        /// <summary>
        /// Creates a function for a blob trigger
        /// </summary>
        /// <typeparam name="TCommand">The type of the command. If the command implements IStreamCommand then
        /// it will be passed the blob as an open stream - otherwise the blob will be deserialized into
        /// the command</typeparam>
        /// <param name="blobPath">Container and optional pattern for the blob</param>
        /// <returns>Builder for use in a fluent API</returns>
        public IStorageFunctionOptionBuilder<TCommand> BlobFunction<TCommand>(string blobPath)
        {
            AbstractFunctionDefinition definition;
            if (typeof(IStreamCommand).IsAssignableFrom(typeof(TCommand)))
            {
                definition = new BlobStreamFunctionDefinition(typeof(TCommand))
                {
                    ConnectionStringName = _connectionName,
                    BlobPath = blobPath
                };
            }
            else
            {
                definition = new BlobFunctionDefinition(typeof(TCommand))
                {
                    ConnectionStringName = _connectionName,
                    BlobPath = blobPath
                };
            }
            
            _definitions.Add(definition);
            return new StorageFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }
    }
}
