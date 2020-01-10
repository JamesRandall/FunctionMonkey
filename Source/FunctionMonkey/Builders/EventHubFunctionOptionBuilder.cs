using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Builders
{
    internal class EventHubFunctionOptionBuilder<TCommandOuter> : IEventHubFunctionOptionBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly IEventHubFunctionBuilder _underlyingBuilder;
        private readonly AbstractFunctionDefinition _functionDefinition;

        public EventHubFunctionOptionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            IEventHubFunctionBuilder underlyingBuilder,
            AbstractFunctionDefinition functionDefinition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _underlyingBuilder = underlyingBuilder;
            _functionDefinition = functionDefinition;
        }
        
        public IEventHubFunctionOptionBuilder<TCommand> EventHubFunction<TCommand>(string eventHubName)
        {
            return _underlyingBuilder.EventHubFunction<TCommand>(eventHubName);
        }

        public IEventHubFunctionOptionBuilder<TCommandOuter> Options(Action<IFunctionOptionsBuilder> options)
        {
            FunctionOptionsBuilder builder = new FunctionOptionsBuilder(_functionDefinition);
            options(builder);
            return this;
        }

        public IOutputBindingBuilder<TCommandOuter, IEventHubFunctionOptionBuilder<TCommandOuter>> OutputTo =>
            new OutputBindingBuilder<TCommandOuter, IEventHubFunctionOptionBuilder<TCommandOuter>>(_connectionStringSettingNames, this, _functionDefinition);
    }
}