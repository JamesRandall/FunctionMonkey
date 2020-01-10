using System;
using System.Collections.Generic;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    public class FunctionBuilder : IFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly List<AbstractFunctionDefinition> _definitions = new List<AbstractFunctionDefinition>();

        public FunctionBuilder(ConnectionStringSettingNames connectionStringSettingNames)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
        }

        public IHttpRouteFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            string rootedRoutePrefix = routePrefix == null ? null : routePrefix.StartsWith("/") ? routePrefix : string.Concat("/", routePrefix);
            HttpRouteConfiguration routeConfiguration = new HttpRouteConfiguration()
            {
                ClaimsPrincipalAuthorizationType = null,
                Route = rootedRoutePrefix,
            };
            HttpFunctionBuilder builder = new HttpFunctionBuilder(_connectionStringSettingNames, routeConfiguration, _definitions);
            httpFunctionBuilder(builder);

            return new HttpRouteFunctionBuilder(this, routeConfiguration);
        }

        public IHttpRouteFunctionBuilder HttpRoute(Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            return HttpRoute(null, httpFunctionBuilder);
        }

        public IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            ServiceBusFunctionBuilder builder = new ServiceBusFunctionBuilder(_connectionStringSettingNames, connectionName, _definitions);
            serviceBusFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder ServiceBus(Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            return ServiceBus(_connectionStringSettingNames.ServiceBus, serviceBusFunctionBuilder);
        }
        
        public IFunctionBuilder EventHub(string connectionName, Action<IEventHubFunctionBuilder> eventHubFunctionBuilder)
        {
            EventHubFunctionBuilder builder = new EventHubFunctionBuilder(_connectionStringSettingNames, connectionName, _definitions);
            eventHubFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder EventHub(Action<IEventHubFunctionBuilder> eventHubFunctionBuilder)
        {
            return EventHub(_connectionStringSettingNames.EventHub, eventHubFunctionBuilder);
        }

        public IFunctionBuilder Storage(string connectionName, Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            StorageFunctionBuilder builder = new StorageFunctionBuilder(_connectionStringSettingNames, connectionName, _definitions);
            storageFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder Storage(Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            return Storage(_connectionStringSettingNames.Storage, storageFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return CosmosDb(_connectionStringSettingNames.CosmosDb, cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(string connectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            CosmosDbFunctionBuilder builder = new CosmosDbFunctionBuilder(_connectionStringSettingNames, connectionName, connectionName, _definitions);
            cosmosDbFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder CosmosDb(Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder, string leaseConnectionName)
        {
            return CosmosDb(_connectionStringSettingNames.CosmosDb, leaseConnectionName, cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(string connectionName, string leaseConnectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            CosmosDbFunctionBuilder builder = new CosmosDbFunctionBuilder(_connectionStringSettingNames, connectionName, leaseConnectionName, _definitions);
            cosmosDbFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder SignalR(string connectionSettingName, Action<ISignalRFunctionBuilder> signalRFunctionBuilder)
        {
            SignalRFunctionBuilder builder = new SignalRFunctionBuilder(_connectionStringSettingNames, connectionSettingName, _definitions);
            signalRFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder SignalR(Action<ISignalRFunctionBuilder> signalRFunctionBuilder)
        {
            SignalRFunctionBuilder builder = new SignalRFunctionBuilder(_connectionStringSettingNames, _connectionStringSettingNames.SignalR, _definitions);
            signalRFunctionBuilder(builder);
            return this;
        }

        public IReadOnlyCollection<HttpFunctionDefinition> GetHttpFunctionDefinitions()
        {
            return _definitions.OfType<HttpFunctionDefinition>().ToArray();
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> Definitions => _definitions;


        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand>(string cronExpression)
        {
            return new TimerFunctionBuilder<TCommand>(_connectionStringSettingNames,this, _definitions).Timer<TCommand>(cronExpression);
        }

        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand, TTimerCommandFactoryType>(string cronExpression) where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>
        {
            return new TimerFunctionBuilder<TCommand>(_connectionStringSettingNames,this, _definitions).Timer<TCommand, TTimerCommandFactoryType>(cronExpression);
        }
    }
}
