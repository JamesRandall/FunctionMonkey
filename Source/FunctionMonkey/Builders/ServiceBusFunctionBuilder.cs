using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    class ServiceBusFunctionBuilder : IServiceBusFunctionBuilder
    {
        private readonly string _connectionName;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public ServiceBusFunctionBuilder(string connectionName, List<AbstractFunctionDefinition> definitions)
        {
            _connectionName = connectionName;
            _definitions = definitions;
        }

        public IServiceBusFunctionOptionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand
        {
            ServiceBusQueueFunctionDefinition definition = new ServiceBusQueueFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                QueueName = queueName
            };
            _definitions.Add(definition);
            return new ServiceBusFunctionOptionBuilder(this, definition);
        }

        public IServiceBusFunctionOptionBuilder SubscriptionFunction<TCommand>(string topicName, string subscriptionName) where TCommand : ICommand
        {
            ServiceBusSubscriptionFunctionDefinition definition =
                new ServiceBusSubscriptionFunctionDefinition(typeof(TCommand))
                {
                    ConnectionStringName = _connectionName,
                    TopicName = topicName,
                    SubscriptionName = subscriptionName
                };
            _definitions.Add(definition);
            return new ServiceBusFunctionOptionBuilder(this, definition);
        }
    }
}
