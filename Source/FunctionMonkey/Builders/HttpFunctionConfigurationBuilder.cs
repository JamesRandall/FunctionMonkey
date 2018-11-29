using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Extensions;
using FunctionMonkey.Http.Abstractions;
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

        public IHttpFunctionConfigurationBuilder<TCommandOuter> AddHeaderMapping<TProperty>(Expression<Func<TCommandOuter, TProperty>> property, string headerName)
        {
            if (_definition.HeaderBindingConfiguration == null)
            {
                _definition.HeaderBindingConfiguration = new HeaderBindingConfiguration();
            }

            MemberInfo member = property.GetMember();
            _definition.HeaderBindingConfiguration.PropertyFromHeaderMappings.Add(member.Name, headerName);

            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> ResponseHandler<TResponseHandler>() where TResponseHandler : IHttpResponseHandler
        {
            _definition.HttpResponseHandlerType = typeof(TResponseHandler);
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> TokenValidator<TTokenValidator>(string header = null) where TTokenValidator : ITokenValidator
        {
            new HttpFunctionOptions(_definition).TokenValidator<TTokenValidator>(header);
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>() where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            new HttpFunctionOptions(_definition).ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>();
            return this;
        }

        public IHttpFunctionConfigurationBuilder<TCommandOuter> Serializer<TSerializer>() where TSerializer : ISerializer
        {
            new HttpFunctionOptions(_definition).Serializer<TSerializer>();
            return this;
        }
        
        public IHttpFunctionConfigurationBuilder<TCommandOuter> JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>() where TSerializerNamingStrategy : NamingStrategy where TDeserializerNamingStrategy : NamingStrategy
        {
            new HttpFunctionOptions(_definition).JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>();
            return this;
        }
    }
}
