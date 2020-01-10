using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    class ServiceBusFunctionBuilder : IServiceBusFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly string _connectionName;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public ServiceBusFunctionBuilder(ConnectionStringSettingNames connectionStringSettingNames, string connectionName, List<AbstractFunctionDefinition> definitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _connectionName = connectionName;
            _definitions = definitions;
        }

        public IServiceBusFunctionOptionBuilder<TCommand> QueueFunction<TCommand>(string queueName, bool isSessionEnabled=false)
        {
            ServiceBusQueueFunctionDefinition definition = new ServiceBusQueueFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                QueueName = queueName,
                IsSessionEnabled = isSessionEnabled
            };
            _definitions.Add(definition);
            return new ServiceBusFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }

        public IServiceBusFunctionOptionBuilder<TCommand> SubscriptionFunction<TCommand>(string topicName, string subscriptionName, bool isSessionEnabled=false)
        {
            ServiceBusSubscriptionFunctionDefinition definition =
                new ServiceBusSubscriptionFunctionDefinition(typeof(TCommand))
                {
                    ConnectionStringName = _connectionName,
                    TopicName = topicName,
                    SubscriptionName = subscriptionName,
                    IsSessionEnabled = isSessionEnabled
                };
            _definitions.Add(definition);
            return new ServiceBusFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }
    }
}
