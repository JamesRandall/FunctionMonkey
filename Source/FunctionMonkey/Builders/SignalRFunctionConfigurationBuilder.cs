using System;
using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class SignalRFunctionConfigurationBuilder<TCommandOuter> : ISignalRFunctionConfigurationBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly ISignalRFunctionBuilder _httpFunctionBuilder;
        private readonly HttpFunctionDefinition _definition;

        public SignalRFunctionConfigurationBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            ISignalRFunctionBuilder httpFunctionBuilder,
            HttpFunctionDefinition definition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _httpFunctionBuilder = httpFunctionBuilder;
            _definition = definition;
        }

        
        public ISignalRFunctionConfigurationBuilder<TCommandOuter> OpenApiDescription(string description)
        {
            _definition.OpenApiDescription = description;
            return this;
        }

        public ISignalRFunctionConfigurationBuilder<TCommandOuter> OpenApiResponse(int httpStatusCode, string description)
        {
            var configuration = new OpenApiResponseConfiguration
            {
                Description = description
            };
            _definition.OpenApiResponseConfigurations.Add(httpStatusCode, configuration);
            return this;
        }

        public ISignalRFunctionConfigurationBuilder<TCommandOuter> Options(Action<IHttpFunctionOptionsBuilder<TCommandOuter>> options)
        {
            HttpFunctionOptionsBuilder<TCommandOuter> builder = new HttpFunctionOptionsBuilder<TCommandOuter>(_definition);
            options(builder);
            return this;
        }
        
        public IOutputBindingBuilder<ISignalRFunctionConfigurationBuilder<TCommandOuter>> OutputTo =>
            new OutputBindingBuilder<ISignalRFunctionConfigurationBuilder<TCommandOuter>>(_connectionStringSettingNames, this, _definition, _pendingOutputConverterType);
        
        private Type _pendingOutputConverterType = null;
        public ISignalRFunctionConfigurationBuilder<TCommandOuter> OutputBindingConverter<TConverter>() where TConverter : IOutputBindingConverter
        {
            if (_definition.OutputBinding != null)
            {
                _definition.OutputBinding.OutputBindingConverterType = typeof(TConverter);
            }
            else
            {
                _pendingOutputConverterType = typeof(TConverter);
            }

            return this;
        }

        public ISignalRFunctionConfigurationBuilder<TCommand> Negotiate<TCommand>(string route, AuthorizationTypeEnum? authorizationType = null,
            params HttpMethod[] method)
        {
            return _httpFunctionBuilder.Negotiate<TCommand>(route, authorizationType, method);
        }

        public ISignalRFunctionBuilder Negotiate(string route, string hubName, string userIdMapping = null,
            AuthorizationTypeEnum? authorizationType = null, params HttpMethod[] method)
        {
            return _httpFunctionBuilder.Negotiate(route, hubName, userIdMapping, authorizationType, method);
        }

        public ISignalRFunctionBuilder NegotiateWithClaim(string route, string hubName, string claimType, params HttpMethod[] method)
        {
            return _httpFunctionBuilder.NegotiateWithClaim(route, hubName, claimType, method);
        }
    }
}
