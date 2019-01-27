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
    internal class HttpFunctionConfigurationBuilder<TCommandOuter> : IHttpFunctionConfigurationBuilder<TCommandOuter> where TCommandOuter : ICommand
    {
        private readonly IHttpFunctionBuilder _httpFunctionBuilder;
        private readonly HttpFunctionDefinition _definition;

        public HttpFunctionConfigurationBuilder(IHttpFunctionBuilder httpFunctionBuilder,
            HttpFunctionDefinition definition)
        {
            _httpFunctionBuilder = httpFunctionBuilder;
            _definition = definition;
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>() where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>();
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(authorizationType);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommand> HttpFunction<TCommand>(string route, AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, authorizationType, method);
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> OpenApiDescription(string description)
        {
            _definition.OpenApiDescription = description;
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> OpenApiResponse(int httpStatusCode, string description)
        {
            _definition.OpenApiResponseDescriptions.Add(httpStatusCode, description);
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> Options(Action<IHttpFunctionOptionsBuilder<TCommandOuter>> options)
        {
            HttpFunctionOptionsBuilder<TCommandOuter> builder = new HttpFunctionOptionsBuilder<TCommandOuter>(_definition);
            options(builder);
            return this;
        }
    }
}
