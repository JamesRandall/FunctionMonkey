using System;
using System.Collections.Generic;
using System.Linq;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class FunctionBuilder : IFunctionBuilder
    {
        private readonly List<AbstractFunctionDefinition> _definitions = new List<AbstractFunctionDefinition>();        

        public IHttpRouteFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            string rootedRoutePrefix = routePrefix.StartsWith("/") ? routePrefix : string.Concat("/", routePrefix);
            HttpRouteConfiguration routeConfiguration = new HttpRouteConfiguration()
            {
                ClaimsPrincipalAuthorizationType = null,
                Route = rootedRoutePrefix,
            };
            HttpFunctionBuilder builder = new HttpFunctionBuilder(routeConfiguration, _definitions);
            httpFunctionBuilder(builder);

            return new HttpRouteFunctionBuilder(this, routeConfiguration);
        }

        public IHttpRouteFunctionBuilder HttpRoute<TAuthorizationType>(string routePrefix,
            Action<IHttpFunctionBuilder> httpFunctionBuilder) where TAuthorizationType : IClaimsPrincipalAuthorization
        {
            string rootedRoutePrefix = routePrefix.StartsWith("/") ? routePrefix : string.Concat("/", routePrefix);
            HttpRouteConfiguration routeConfiguration = new HttpRouteConfiguration()
            {
                ClaimsPrincipalAuthorizationType = typeof(TAuthorizationType),
                Route = rootedRoutePrefix,
            };
            HttpFunctionBuilder builder = new HttpFunctionBuilder(routeConfiguration, _definitions);
            httpFunctionBuilder(builder);

            return new HttpRouteFunctionBuilder(this, routeConfiguration);
        }

        public IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            ServiceBusFunctionBuilder builder = new ServiceBusFunctionBuilder(connectionName, _definitions);
            serviceBusFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder Storage(string connectionName, Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            StorageFunctionBuilder builder = new StorageFunctionBuilder(connectionName, _definitions);
            storageFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder CosmosDb(string connectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            CosmosDbFunctionBuilder builder = new CosmosDbFunctionBuilder(connectionName, connectionName, _definitions);
            cosmosDbFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder CosmosDb(string connectionName, string leaseConnectionName, Action<ICosmosDbFunctionBuilder> cosmosDbFunctionBuilder)
        {
            CosmosDbFunctionBuilder builder = new CosmosDbFunctionBuilder(connectionName, leaseConnectionName, _definitions);
            cosmosDbFunctionBuilder(builder);
            return this;
        }

        public IReadOnlyCollection<HttpFunctionDefinition> GetHttpFunctionDefinitions()
        {
            return _definitions.OfType<HttpFunctionDefinition>().ToArray();
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> Definitions => _definitions;


        public IFunctionBuilder Timer<TCommand>(string cronExpression) where TCommand : ICommand
        {
            return new TimerFunctionBuilder(this, _definitions).Timer<TCommand>(cronExpression);
        }

        public IFunctionBuilder Timer<TCommand, TTimerCommandFactoryType>(string cronExpression) where TCommand : ICommand where TTimerCommandFactoryType : ITimerCommandFactory<TCommand>
        {
            return new TimerFunctionBuilder(this, _definitions).Timer<TCommand, TTimerCommandFactoryType>(cronExpression);
        }        
    }
}
