using System;
using System.Linq.Expressions;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Extensions;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    public class HttpFunctionOptionsBuilder<TCommand> : IHttpFunctionOptionsBuilder<TCommand>
    {
        private readonly HttpFunctionDefinition _functionDefinition;

        public HttpFunctionOptionsBuilder(HttpFunctionDefinition functionDefinition)
        {
            _functionDefinition = functionDefinition;
        }
        
        public IHttpFunctionOptionsBuilder<TCommand> Serializer<TSerializer>() where TSerializer : ISerializer
        {
            _functionDefinition.CommandDeserializerType = typeof(TSerializer);
            return this;
        }

        public IHttpFunctionOptionsBuilder<TCommand> JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>() where TDeserializerNamingStrategy : NamingStrategy where TSerializerNamingStrategy : NamingStrategy
        {
            _functionDefinition.DeserializerNamingStrategyType = typeof(TDeserializerNamingStrategy);
            _functionDefinition.SerializerNamingStrategyType = typeof(TSerializerNamingStrategy);
            return this;
        }

        public IHttpFunctionOptionsBuilder<TCommand> TokenValidator<TTokenValidator>(string header = null) where TTokenValidator : ITokenValidator
        {
            _functionDefinition.TokenValidatorType = typeof(TTokenValidator);
            if (header != null)
            {
                _functionDefinition.TokenHeader = header;
            }

            return this;
        }

        public IHttpFunctionOptionsBuilder<TCommand> ClaimsPrincipalAuthorization<TClaimsPrincipalAuthorization>() where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            _functionDefinition.ClaimsPrincipalAuthorizationType = typeof(TClaimsPrincipalAuthorization);
            return this;
        }

        public IHttpFunctionOptionsBuilder<TCommand> ResponseHandler<TResponseHandler>() where TResponseHandler : IHttpResponseHandler
        {
            _functionDefinition.HttpResponseHandlerType = typeof(TResponseHandler);
            return this;
        }

        public IHttpFunctionOptionsBuilder<TCommand> AddHeaderMapping<TProperty>(Expression<Func<TCommand, TProperty>> property, string headerName)
        {
            if (_functionDefinition.HeaderBindingConfiguration == null)
            {
                _functionDefinition.HeaderBindingConfiguration = new HeaderBindingConfiguration();
            }

            MemberInfo member = property.GetMember();
            _functionDefinition.HeaderBindingConfiguration.PropertyFromHeaderMappings.Add(member.Name, headerName);

            return this;
        }

        public IHttpFunctionOptionsBuilder<TCommand> NoCommandHandler()
        {
            _functionDefinition.NoCommandHandler = true;
            return this;
        }
    }
}