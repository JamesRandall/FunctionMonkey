using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Builders
{
    internal class TimerFunctionOptionsBuilder<TCommandOuter> : ITimerFunctionOptionsBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly IFunctionBuilder _functionBuilder;
        private readonly AbstractFunctionDefinition _functionDefinition;

        public TimerFunctionOptionsBuilder(ConnectionStringSettingNames connectionStringSettingNames, IFunctionBuilder functionBuilder, AbstractFunctionDefinition functionDefinition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _functionBuilder = functionBuilder;
            _functionDefinition = functionDefinition;
        }

        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand>(string cronExpression)
        {
            return _functionBuilder.Timer<TCommand>(cronExpression);
        }

        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand, TTimerCommandFactoryType>(string cronExpression) where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>
        {
            return _functionBuilder.Timer<TCommand, TTimerCommandFactoryType>(cronExpression);
        }

        public IHttpRouteFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            return _functionBuilder.HttpRoute(routePrefix, httpFunctionBuilder);
        }

        public IHttpRouteFunctionBuilder HttpRoute(Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            return _functionBuilder.HttpRoute(httpFunctionBuilder);
        }

        public IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            return _functionBuilder.ServiceBus(connectionName, serviceBusFunctionBuilder);
        }

        public IFunctionBuilder ServiceBus(Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            return _functionBuilder.ServiceBus(serviceBusFunctionBuilder);
        }

        public IFunctionBuilder EventHub(string connectionName, Action<IEventHubFunctionBuilder> eventHubFunctionBuilder)
        {
            return _functionBuilder.EventHub(connectionName, eventHubFunctionBuilder);
        }

        public IFunctionBuilder EventHub(Action<IEventHubFunctionBuilder> eventHubFunctionBuilder)
        {
            return _functionBuilder.EventHub(eventHubFunctionBuilder);
        }

        public IFunctionBuilder Storage(string connectionName, Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            return _functionBuilder.Storage(connectionName, storageFunctionBuilder);
        }

        public IFunctionBuilder Storage(Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            return _functionBuilder.Storage(storageFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return _functionBuilder.CosmosDb(cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(string connectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return _functionBuilder.CosmosDb(connectionName, cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder, string leaseConnectionName)
        {
            return _functionBuilder.CosmosDb(cosmosDbFunctionBuilder, leaseConnectionName);
        }

        public IFunctionBuilder CosmosDb(string connectionName, string leaseConnectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return _functionBuilder.CosmosDb(connectionName, leaseConnectionName, cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder SignalR(string connectionSettingName, Action<ISignalRFunctionBuilder> signalRFunctionBuilder)
        {
            return _functionBuilder.SignalR(connectionSettingName, signalRFunctionBuilder);
        }

        public IFunctionBuilder SignalR(Action<ISignalRFunctionBuilder> signalRFunctionBuilder)
        {
            return _functionBuilder.SignalR(signalRFunctionBuilder);
        }

        public IOutputBindingBuilder<IFunctionBuilder> OutputTo => new OutputBindingBuilder<IFunctionBuilder>(_connectionStringSettingNames, _functionBuilder, _functionDefinition, _pendingOutputConverterType);
        
        private Type _pendingOutputConverterType = null;
        public IFunctionBuilder OutputBindingConverter<TConverter>() where TConverter : IOutputBindingConverter
        {
            if (_functionDefinition.OutputBinding != null)
            {
                _functionDefinition.OutputBinding.OutputBindingConverterType = typeof(TConverter);
            }
            else
            {
                _pendingOutputConverterType = typeof(TConverter);
            }

            return this;
        }
    }
}
