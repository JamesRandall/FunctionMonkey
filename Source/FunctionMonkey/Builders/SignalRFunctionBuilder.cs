using System;
using System.Collections.Generic;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class SignalRFunctionBuilder : ISignalRFunctionBuilder
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly string _connectionStringSettingName;
        private readonly List<AbstractFunctionDefinition> _definitions;

        public SignalRFunctionBuilder(ConnectionStringSettingNames connectionStringSettingNames, string connectionStringSettingName, List<AbstractFunctionDefinition> definitions)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _connectionStringSettingName = connectionStringSettingName;
            _definitions = definitions;
        }

        public ISignalRFunctionConfigurationBuilder<TCommand> Negotiate<TCommand>(string route, AuthorizationTypeEnum? authorizationType = null,
            params HttpMethod[] method)
        {
            SignalRCommandNegotiateFunctionDefinition definition = new SignalRCommandNegotiateFunctionDefinition(typeof(TCommand))
            {
                ConnectionStringSettingName = _connectionStringSettingName,
                SubRoute = route,
                RouteConfiguration = new HttpRouteConfiguration
                {
                    Route = route
                },
                Route = route,
                Verbs = new HashSet<HttpMethod>(method),
                Authorization = authorizationType
            };
            _definitions.Add(definition);

            return new SignalRFunctionConfigurationBuilder<TCommand>(_connectionStringSettingNames, this, definition);
        }

        public ISignalRFunctionBuilder Negotiate(string route, string hubName, string userIdExpression = null,
            AuthorizationTypeEnum? authorizationType = null, params HttpMethod[] method)
        {
            SignalRBindingExpressionNegotiateFunctionDefinition definition =
                new SignalRBindingExpressionNegotiateFunctionDefinition()
                {
                    ConnectionStringSettingName = _connectionStringSettingName,
                    SubRoute = route,
                    RouteConfiguration = new HttpRouteConfiguration
                    {
                        Route = route
                    },
                    Route = route,
                    Verbs = new HashSet<HttpMethod>(method),
                    Authorization = authorizationType,
                    HubName = hubName,
                    UserIdExpression = userIdExpression
                };
            _definitions.Add(definition);
            return new SignalRFunctionConfigurationBuilder<SignalRBindingExpressionNegotiateCommand>(_connectionStringSettingNames, this, definition);
        }
        
        public ISignalRFunctionBuilder NegotiateWithClaim(string route, string hubName, string claimType, params HttpMethod[] method)
        {
            SignalRClaimNegotiateFunctionDefinition definition =
                new SignalRClaimNegotiateFunctionDefinition()
                {
                    ConnectionStringSettingName = _connectionStringSettingName,
                    SubRoute = route,
                    RouteConfiguration = new HttpRouteConfiguration
                    {
                        Route = route
                    },
                    Route = route,
                    Verbs = new HashSet<HttpMethod>(method),
                    Authorization = AuthorizationTypeEnum.TokenValidation,
                    HubName = hubName,
                    ClaimType = claimType
                };
            _definitions.Add(definition);
            return new SignalRFunctionConfigurationBuilder<SignalRBindingExpressionNegotiateCommand>(_connectionStringSettingNames, this, definition);
        }
    }
}
