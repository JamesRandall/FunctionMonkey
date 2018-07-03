using System;
using System.Collections.Generic;
using System.Linq;
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


        public IReadOnlyCollection<HttpFunctionDefinition> GetHttpFunctionDefinitions()
        {
            return _definitions.OfType<HttpFunctionDefinition>().ToArray();
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> Definitions => _definitions;        
    }
}
