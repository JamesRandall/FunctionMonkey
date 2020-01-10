using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    internal class ServiceBusFunctionOptionBuilder<TCommandOuter> : IServiceBusFunctionOptionBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly IServiceBusFunctionBuilder _underlyingBuilder;
        private readonly AbstractFunctionDefinition _functionDefinition;

        public ServiceBusFunctionOptionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            IServiceBusFunctionBuilder underlyingBuilder,
            AbstractFunctionDefinition functionDefinition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _underlyingBuilder = underlyingBuilder;
            _functionDefinition = functionDefinition;
        }
        
        public IServiceBusFunctionOptionBuilder<TCommand> QueueFunction<TCommand>(string queueName, bool isSessionEnabled=false)
        {
            return _underlyingBuilder.QueueFunction<TCommand>(queueName, isSessionEnabled);
        }

        public IServiceBusFunctionOptionBuilder<TCommand> SubscriptionFunction<TCommand>(string topicName, string subscriptionName, bool isSessionEnabled=false)
        {
            return _underlyingBuilder.SubscriptionFunction<TCommand>(topicName, subscriptionName, isSessionEnabled);
        }

        public IServiceBusFunctionOptionBuilder<TCommandOuter> Serializer<TSerializer>() where TSerializer : ISerializer
        {
            new FunctionOptions(_functionDefinition).Serializer<TSerializer>();
            return this;
        }


        public IServiceBusFunctionOptionBuilder<TCommandOuter> Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_functionDefinition);
            options(builder);
            return this;
        }
        
        public IOutputBindingBuilder<TCommandOuter, IServiceBusFunctionOptionBuilder<TCommandOuter>> OutputTo =>
            new OutputBindingBuilder<TCommandOuter, IServiceBusFunctionOptionBuilder<TCommandOuter>>(_connectionStringSettingNames, this, _functionDefinition);
    }
}