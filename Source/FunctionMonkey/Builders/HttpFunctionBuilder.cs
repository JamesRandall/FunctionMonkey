using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    class HttpFunctionBuilder : IHttpFunctionBuilder
    {
        private static readonly HttpMethod DefaultMethod = HttpMethod.Get;
        private readonly HttpRouteConfiguration _routeConfiguration;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public HttpFunctionBuilder(HttpRouteConfiguration routeConfiguration, List<AbstractFunctionDefinition> definitions)
        {
            _routeConfiguration = routeConfiguration;
            _definitions = definitions;
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>() where TCommand : ICommand
        {
            return HttpFunction<TCommand>((string)null, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand
        {
            return HttpFunction<TCommand>((string)null, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route) where TCommand : ICommand
        {
            return HttpFunction<TCommand>(route, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand
        {
            HttpFunctionDefinition definition = new HttpFunctionDefinition(typeof(TCommand))
            {
                SubRoute = route,
                RouteConfiguration = _routeConfiguration,
                Route = string.Concat(_routeConfiguration.Route, route),
                Verbs = new HashSet<HttpMethod>(method)
            };
            _definitions.Add(definition);
            return new HttpFunctionBuilderMetadataBuilder(this, definition);
        }        
    }
}
