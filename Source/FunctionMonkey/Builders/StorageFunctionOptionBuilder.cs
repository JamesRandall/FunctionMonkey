using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class StorageFunctionOptionBuilder<TCommandOuter> : IStorageFunctionOptionBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly IStorageFunctionBuilder _underlyingBuilder;
        private readonly AbstractFunctionDefinition _definition;

        public StorageFunctionOptionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            IStorageFunctionBuilder underlyingBuilder,
            AbstractFunctionDefinition definition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _underlyingBuilder = underlyingBuilder;
            _definition = definition;
        }
        
        public IStorageFunctionOptionBuilder<TCommand> QueueFunction<TCommand>(string queueName)
        {
            return _underlyingBuilder.QueueFunction<TCommand>(queueName);
        }

        public IStorageFunctionOptionBuilder<TCommand> BlobFunction<TCommand>(string blobPath)
        {
            return _underlyingBuilder.BlobFunction<TCommand>(blobPath);
        }


        public IStorageFunctionOptionBuilder<TCommandOuter> Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_definition);
            options(builder);
            return this;
        }
        
        public IOutputBindingBuilder<TCommandOuter, IStorageFunctionOptionBuilder<TCommandOuter>> OutputTo =>
            new OutputBindingBuilder<TCommandOuter, IStorageFunctionOptionBuilder<TCommandOuter>>(_connectionStringSettingNames, this, _definition);
    }
}