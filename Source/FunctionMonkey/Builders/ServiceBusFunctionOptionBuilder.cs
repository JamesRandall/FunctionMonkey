using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class ServiceBusFunctionOptionBuilder : IServiceBusFunctionOptionBuilder
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
        
        public IServiceBusFunctionOptionBuilder QueueFunction<TCommand>(string queueName, bool isSessionEnabled=false) where TCommand : ICommand
        {
            return _underlyingBuilder.QueueFunction<TCommand>(queueName, isSessionEnabled);
        }

        public IServiceBusFunctionOptionBuilder SubscriptionFunction<TCommand>(string topicName, string subscriptionName, bool isSessionEnabled=false) where TCommand : ICommand
        {
            return _underlyingBuilder.SubscriptionFunction<TCommand>(topicName, subscriptionName, isSessionEnabled);
        }

        public IServiceBusFunctionOptionBuilder Serializer<TSerializer>() where TSerializer : ISerializer
        {
            new FunctionOptions(_functionDefinition).Serializer<TSerializer>();
            return this;
        }


        public IServiceBusFunctionOptionBuilder Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_functionDefinition);
            options(builder);
            return this;
        }
        
        public IOutputBindingBuilder<IServiceBusFunctionOptionBuilder> OutputTo =>
            new OutputBindingBuilder<IServiceBusFunctionOptionBuilder>(_connectionStringSettingNames, this, _functionDefinition);
    }
}