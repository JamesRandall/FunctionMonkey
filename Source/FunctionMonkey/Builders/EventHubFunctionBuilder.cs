using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class EventHubFunctionBuilder : IEventHubFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly string _connectionName;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public EventHubFunctionBuilder(ConnectionStringSettingNames connectionStringSettingNames, string connectionName, List<AbstractFunctionDefinition> definitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _connectionName = connectionName;
            _definitions = definitions;
        }
        
        public IEventHubFunctionOptionBuilder<TCommand> EventHubFunction<TCommand>(string eventHubName)
        {
            EventHubFunctionDefinition definition = new EventHubFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringName = _connectionName,
                EventHubName = eventHubName
            };
            _definitions.Add(definition);
            return new EventHubFunctionOptionBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }
    }
}