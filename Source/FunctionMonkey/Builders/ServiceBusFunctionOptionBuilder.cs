using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class ServiceBusFunctionOptionBuilder : IServiceBusFunctionOptionBuilder
    {
        private readonly IServiceBusFunctionBuilder _underlyingBuilder;
        private readonly AbstractFunctionDefinition _functionDefinition;

        public ServiceBusFunctionOptionBuilder(IServiceBusFunctionBuilder underlyingBuilder,
            AbstractFunctionDefinition functionDefinition)
        {
            _underlyingBuilder = underlyingBuilder;
            _functionDefinition = functionDefinition;
        }
        
        public IServiceBusFunctionOptionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand
        {
            return _underlyingBuilder.QueueFunction<TCommand>(queueName);
        }

        public IServiceBusFunctionOptionBuilder SubscriptionFunction<TCommand>(string topicName, string subscriptionName) where TCommand : ICommand
        {
            return _underlyingBuilder.SubscriptionFunction<TCommand>(topicName, subscriptionName);
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
    }
}