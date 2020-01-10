using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    class HttpFunctionBuilder : IHttpFunctionBuilder
    {
        private static readonly HttpMethod DefaultMethod = HttpMethod.Get;

        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly HttpRouteConfiguration _routeConfiguration;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public HttpFunctionBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            HttpRouteConfiguration routeConfiguration,
            List<AbstractFunctionDefinition> definitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _routeConfiguration = routeConfiguration;
            _definitions = definitions;
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>()
        {
            return BuildHttpFunction<TCommand>(null, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType)
        {
            return BuildHttpFunction<TCommand>(null, authorizationType, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method)
        {
            return BuildHttpFunction<TCommand>(null, authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(params HttpMethod[] method)
        {
            return BuildHttpFunction<TCommand>(null, null, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route)
        {
            return BuildHttpFunction<TCommand>(route, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, params HttpMethod[] method)
        {
            return BuildHttpFunction<TCommand>(route, null, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route,
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method)
        {
            return BuildHttpFunction<TCommand>(route, authorizationType, method);
        }

        private IHttpFunctionConfigurationBuilder<TCommand> BuildHttpFunction<TCommand>(string route,
            AuthorizationTypeEnum? authorizationType,
            params HttpMethod[] method)
        {
            HttpFunctionDefinition definition = new HttpFunctionDefinition(typeof(TCommand))
            {
                SubRoute = route,
                RouteConfiguration = _routeConfiguration,
                Route = _routeConfiguration.Route == null ? route : string.Concat(_routeConfiguration.Route, route).TrimStart('/'),
                Verbs = new HashSet<HttpMethod>(method),
                Authorization = authorizationType,
                ClaimsPrincipalAuthorizationType = _routeConfiguration.ClaimsPrincipalAuthorizationType
            };
            _definitions.Add(definition);
            return new HttpFunctionConfigurationBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }
    }
}
