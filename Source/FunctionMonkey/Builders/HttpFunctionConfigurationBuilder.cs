using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    internal class HttpFunctionConfigurationBuilder<TCommandOuter> : IHttpFunctionConfigurationBuilder<TCommandOuter>
    {
        private readonly ConnectionStringSettingNames _connectionStringSettingNames;
        private readonly IHttpFunctionBuilder _httpFunctionBuilder;
        private readonly HttpFunctionDefinition _definition;

        public HttpFunctionConfigurationBuilder(
            ConnectionStringSettingNames connectionStringSettingNames,
            IHttpFunctionBuilder httpFunctionBuilder,
            HttpFunctionDefinition definition)
        {
            _connectionStringSettingNames = connectionStringSettingNames;
            _httpFunctionBuilder = httpFunctionBuilder;
            _definition = definition;
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>()
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>();
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType)
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(authorizationType);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method)
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(params HttpMethod[] method)
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, params HttpMethod[] method)
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method)
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> OpenApiDescription(string description)
        {
            _definition.OpenApiDescription = description;
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> OpenApiSummary(string summary)
        {
            _definition.OpenApiSummary = summary;
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> OpenApiResponse(int httpStatusCode, string description, Type responseType = null)
        {
            var configuration = new OpenApiResponseConfiguration
            {
                Description = description,
                ResponseType = responseType
            };
            _definition.OpenApiResponseConfigurations.Add(httpStatusCode, configuration);
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> Options(Action<IHttpFunctionOptionsBuilder<TCommandOuter>> options)
        {
            HttpFunctionOptionsBuilder<TCommandOuter> builder = new HttpFunctionOptionsBuilder<TCommandOuter>(_definition);
            options(builder);
            return this;
        }
        
        public IOutputBindingBuilder<IHttpFunctionConfigurationBuilder<TCommandOuter>> OutputTo =>
            new OutputBindingBuilder<IHttpFunctionConfigurationBuilder<TCommandOuter>>(_connectionStringSettingNames, this, _definition, _pendingOutputConverterType);

        private Type _pendingOutputConverterType = null;
        public IHttpFunctionConfigurationBuilder<TCommandOuter> OutputBindingConverter<TConverter>() where TConverter : IOutputBindingConverter
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
    }
}
