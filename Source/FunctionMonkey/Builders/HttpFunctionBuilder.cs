using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
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
            return BuildHttpFunction<TCommand>(null, null, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, authorizationType, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(null, null, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(route, null, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(route, null, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route,
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand
        {
            return BuildHttpFunction<TCommand>(route, authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>() where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, null, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(
            AuthorizationTypeEnum authorizationType) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, authorizationType, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(null, null, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route,
            params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(route, null, DefaultMethod);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route,
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(route, authorizationType, method);
        }

        private IHttpFunctionBuilderMetadataBuilder BuildHttpFunction<TCommand>(string route, AuthorizationTypeEnum? authorizationType, params HttpMethod[] method) where TCommand : ICommand
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
            return new HttpFunctionBuilderMetadataBuilder(this, definition);
        }

        private IHttpFunctionBuilderMetadataBuilder BuildHttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route, AuthorizationTypeEnum? authorizationType, params HttpMethod[] method) where TCommand : ICommand
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
            return new HttpFunctionBuilderMetadataBuilder(this, definition);
        }
    }
}
