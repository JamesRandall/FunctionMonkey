using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
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

        public IServiceBusFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand
        {
            _definitions.Add(new ServiceBusQueueFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                QueueName = queueName
            });
            return this;
        }

        public IServiceBusFunctionBuilder SubscriptionFunction<TCommand>(string topicName, string subscriptionName) where TCommand : ICommand
        {
            _definitions.Add(new ServiceBusSubscriptionFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                TopicName = topicName,
                SubscriptionName = subscriptionName
            });
            return this;
        }
    }
}
