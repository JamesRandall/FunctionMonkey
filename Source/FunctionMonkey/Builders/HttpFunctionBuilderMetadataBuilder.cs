using System.Net.Http;
using AzureFromTheTrenches.Commanding.Abstractions;
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

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(method);
        }

        public IHttpFunctionBuilderMetadataBuilder HttpFunction<TCommand>(string route, params HttpMethod[] method) where TCommand : ICommand
        {
            return _httpFunctionBuilder.HttpFunction<TCommand>(route, method);
        }

        public IHttpFunctionBuilderMetadataBuilder Description(string description)
        {
            _definition.OpenApiDescription = description;
            return this;
        }

        public IHttpFunctionBuilderMetadataBuilder Response(int httpStatusCode, string description)
        {
            _definition.OpenApiResponseDescriptions.Add(httpStatusCode, description);
            return this;
        }
    }
}
