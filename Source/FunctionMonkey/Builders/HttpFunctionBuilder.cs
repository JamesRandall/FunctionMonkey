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

        private readonly HttpRouteConfiguration _routeConfiguration;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public HttpFunctionBuilder(HttpRouteConfiguration routeConfiguration, List<AbstractFunctionDefinition> definitions)
        {
            _routeConfiguration = routeConfiguration;
            _definitions = definitions;
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>() where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, null, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, authorizationType, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, null, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(route, null, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(route, null, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route,
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(route, authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand, TClaimsPrincipalAuthorization>() where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand, TClaimsPrincipalAuthorization>(
            AuthorizationTypeEnum authorizationType) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, authorizationType, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand, TClaimsPrincipalAuthorization>(
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand, TClaimsPrincipalAuthorization>(params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, null, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route,
            params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(route, null, DefaultMethod);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route,
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(route, authorizationType, method);
        }

        private IHttpFunctionConfigurationBuilder<TCommand> BuildHttpFunction<TCommand>(string route,
            AuthorizationTypeEnum? authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            HttpFunctionDefinition definition = new HttpFunctionDefinition(typeof(TCommand))
            {
                SubRoute = route,
                RouteConfiguration = _routeConfiguration,
                Route = string.Concat(_routeConfiguration.Route, route),
                Verbs = new HashSet<HttpMethod>(method),
                Authorization = authorizationType,
                ClaimsPrincipalAuthorizationType = _routeConfiguration.ClaimsPrincipalAuthorizationType
            };
            _definitions.Add(definition);
            return new HttpFunctionConfigurationBuilder<TCommand>(this, definition);
        }

        private IHttpFunctionConfigurationBuilder<TCommand> BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route, AuthorizationTypeEnum? authorizationType, params HttpMethod[] method) where TCommand : ICommand
        {
            HttpFunctionDefinition definition = new HttpFunctionDefinition(typeof(TCommand))
            {
                SubRoute = route,
                RouteConfiguration = _routeConfiguration,
                Route = string.Concat(_routeConfiguration.Route, route),
                Verbs = new HashSet<HttpMethod>(method),
                Authorization = authorizationType,
                ClaimsPrincipalAuthorizationType = typeof(TClaimsPrincipalAuthorization)
            };
            _definitions.Add(definition);
            return new HttpFunctionConfigurationBuilder<TCommand>(this, definition);
        }
    }
}
