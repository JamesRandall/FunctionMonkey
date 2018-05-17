using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions;
using AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders;
using AzureFromTheTrenches.Commanding.AzureFunctions.Model;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Builders
{
    internal class FunctionBuilder : IFunctionBuilder
    {
        private readonly List<AbstractFunctionDefinition> _definitions = new List<AbstractFunctionDefinition>();

        public IFunctionBuilder HttpRoute(string routePrefix, Action<IHttpFunctionBuilder> httpRouteBuilder)
        {
            return HttpRoute(routePrefix, null, null, httpRouteBuilder);
        }

        public IFunctionBuilder HttpRoute(string routePrefix, string openApiName,
            Action<IHttpFunctionBuilder> httpRouteBuilder)
        {
            return HttpRoute(routePrefix, openApiName, null, httpRouteBuilder);
        }

        public IFunctionBuilder HttpRoute(string routePrefix, string openApiName, string openApiDescription, Action<IHttpFunctionBuilder> httpFunctionBuilder)
        {
            HttpRouteConfiguration routeConfiguration = new HttpRouteConfiguration()
            {
                Route = routePrefix,
                OpenApiDescription = openApiDescription,
                OpenApiName = openApiName
            };
            HttpFunctionBuilder builder = new HttpFunctionBuilder(routeConfiguration, _definitions);
            httpFunctionBuilder(builder);
            return this;
        }

        public IFunctionBuilder ServiceBus(string connectionName, Action<IServiceBusFunctionBuilder> serviceBusFunctionBuilder)
        {
            ServiceBusFunctionBuilder builder = new ServiceBusFunctionBuilder(connectionName, _definitions);
            serviceBusFunctionBuilder(builder);
            return this;
        }

        /*public IFunctionBuilder StorageQueueFunction<TCommand>() where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>(string functionName) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>(Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public IFunctionBuilder StorageQueueFunction<TCommand>(string functionName, Action<IStorageQueueFunctionBuilder> storageQueueFunctionBuilder) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }*/
        
        public IReadOnlyCollection<HttpFunctionDefinition> GetHttpFunctionDefinitions()
        {
            return _definitions.OfType<HttpFunctionDefinition>().ToArray();
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> Definitions => _definitions;        
    }
}
