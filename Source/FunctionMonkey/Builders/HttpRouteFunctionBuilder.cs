using System;
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

        public IFunctionBuilder Storage(string connectionName, Action<IStorageFunctionBuilder> storageFunctionBuilder)
        {
            return _decoratedBuilder.Storage(connectionName, storageFunctionBuilder);
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
    }
}
