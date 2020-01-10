using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class HttpRouteFunctionBuilder : IHttpRouteFunctionBuilder
    {
        private readonly IFunctionBuilder _decoratedBuilder;
        private readonly HttpRouteConfiguration _httpRouteConfiguration;

        public HttpRouteFunctionBuilder(IFunctionBuilder decoratedBuilder,
            HttpRouteConfiguration httpRouteConfiguration)
        {
            _decoratedBuilder = decoratedBuilder;
            _httpRouteConfiguration = httpRouteConfiguration;
        }

        public IHttpRouteFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            return _decoratedBuilder.HttpRoute(routePrefix, httpFunctionBuilder);
        }

        public IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            return _decoratedBuilder.ServiceBus(connectionName, serviceBusFunctionBuilder);
        }

        public IFunctionBuilder ServiceBus(Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            return _decoratedBuilder.ServiceBus(serviceBusFunctionBuilder);
        }

        public IFunctionBuilder EventHub(string connectionName, Action<IEventHubFunctionBuilder> eventHubFunctionBuilder)
        {
            return _decoratedBuilder.EventHub(connectionName, eventHubFunctionBuilder);
        }

        public IFunctionBuilder EventHub(Action<IEventHubFunctionBuilder> eventHubFunctionBuilder)
        {
            return _decoratedBuilder.EventHub(eventHubFunctionBuilder);
        }

        public IFunctionBuilder Storage(string connectionName, Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            return _decoratedBuilder.Storage(connectionName, storageFunctionBuilder);
        }

        public IFunctionBuilder Storage(Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            return _decoratedBuilder.Storage(storageFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return _decoratedBuilder.CosmosDb(cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(string connectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return _decoratedBuilder.CosmosDb(connectionName, cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder CosmosDb(Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder, string leaseConnectionName)
        {
            return _decoratedBuilder.CosmosDb(cosmosDbFunctionBuilder, leaseConnectionName);
        }

        public IFunctionBuilder CosmosDb(string connectionName, string leaseConnectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            return _decoratedBuilder.CosmosDb(connectionName, leaseConnectionName, cosmosDbFunctionBuilder);
        }

        public IFunctionBuilder SignalR(string connectionSettingName, Action<ISignalRFunctionBuilder> signalRFunctionBuilder)
        {
            return _decoratedBuilder.SignalR(connectionSettingName, signalRFunctionBuilder);
        }

        public IFunctionBuilder SignalR(Action<ISignalRFunctionBuilder> signalRFunctionBuilder)
        {
            return _decoratedBuilder.SignalR(signalRFunctionBuilder);
        }

        public IHttpRouteFunctionBuilder OpenApiDescription(string description)
        {
            _httpRouteConfiguration.OpenApiDescription = description;
            return this;
        }

        public IHttpRouteFunctionBuilder OpenApiName(string name)
        {
            _httpRouteConfiguration.OpenApiName = name;
            return this;
        }

        public IHttpRouteFunctionBuilder Options(Action<IHttpRouteOptionsBuilder> options)
        {
            IHttpRouteOptionsBuilder builder = new HttpRouteOptionsBuilder(_httpRouteConfiguration);
            options(builder);
            return this;
        }

        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand>(string cronExpression)
        {
            return _decoratedBuilder.Timer<TCommand>(cronExpression);
        }

        public ITimerFunctionOptionsBuilder<TCommand> Timer<TCommand, TTimerCommandFactoryType>(string cronExpression) where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>
        {
            return _decoratedBuilder.Timer<TCommand, TTimerCommandFactoryType>(cronExpression);
        }

        public IHttpRouteFunctionBuilder HttpRoute(Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            return _decoratedBuilder.HttpRoute(httpFunctionBuilder);
        }
    }
}
