using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class StorageFunctionOptionBuilder : IStorageFunctionOptionBuilder
    {
        private readonly IStorageFunctionBuilder _underlyingBuilder;
        private readonly AbstractFunctionDefinition _definition;

        public StorageFunctionOptionBuilder(IStorageFunctionBuilder underlyingBuilder,
            AbstractFunctionDefinition definition)
        {
            _underlyingBuilder = underlyingBuilder;
            _definition = definition;
        }
        
        public IStorageFunctionOptionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand
        {
            return _underlyingBuilder.QueueFunction<TCommand>(queueName);
        }

        public IStorageFunctionOptionBuilder BlobFunction<TCommand>(string blobPath) where TCommand : ICommand
        {
            return _underlyingBuilder.BlobFunction<TCommand>(blobPath);
        }


        public IStorageFunctionOptionBuilder Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_definition);
            options(builder);
            return this;
        }
    }
}