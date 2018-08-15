using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Model;

namespace FunctionMonkey.Builders
{
    internal class HttpFunctionBuilderMetadataBuilder : IHttpFunctionBuilderMetadataBuilder
    {
        private readonly IHttpFunctionBuilder _httpFunctionBuilder;
        private readonly HttpFunctionDefinition _definition;

        public HttpFunctionBuilderMetadataBuilder(IHttpFunctionBuilder httpFunctionBuilder,
            HttpFunctionDefinition definition)
        {
            _httpFunctionBuilder = httpFunctionBuilder;
            _definition = definition;
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>() where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>();
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(authorizationType);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, AuthorizationTypeEnum authorizationType,
            params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>() where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return _httpFunctionBuilder.HttpFunction<TCommand, TClaimsPrincipalAuthorization>();
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(
            AuthorizationTypeEnum authorizationType) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return _httpFunctionBuilder.HttpFunction<TCommand, TClaimsPrincipalAuthorization>(authorizationType);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return _httpFunctionBuilder.HttpFunction<TCommand, TClaimsPrincipalAuthorization>(authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return _httpFunctionBuilder.HttpFunction<TCommand, TClaimsPrincipalAuthorization>(method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route,
            params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return _httpFunctionBuilder.HttpFunction<TCommand, TClaimsPrincipalAuthorization>(route, method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand, TClaimsPrincipalAuthorization>(string route,
            AuthorizationTypeEnum authorizationType, params HttpMethod[] method) where TCommand : ICommand where TClaimsPrincipalAuthorization : IClaimsPrincipalAuthorization
        {
            return _httpFunctionBuilder.HttpFunction<TCommand, TClaimsPrincipalAuthorization>(route, authorizationType, method);
        }

        public IHttpFunctionBuilderMetadataBuilder OpenApiDescription(string description)
        {
            _definition.OpenApiDescription = description;
            return this;
        }

        public IHttpFunctionBuilderMetadataBuilder OpenApiResponse(int httpStatusCode, string description)
        {
            _definition.OpenApiResponseDescriptions.Add(httpStatusCode, description);
            return this;
        }
    }
}
