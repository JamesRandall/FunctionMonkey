using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Builders;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Testing
{
    public class TestFunctionHostBuilder : IFunctionHostBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ICommandRegistry _commandRegistry;
        private readonly FunctionBuilder _functionBuilder = new FunctionBuilder();

        
        public TestFunctionHostBuilder(IServiceCollection serviceCollection, ICommandRegistry commandRegistry)
        {
            _serviceCollection = serviceCollection;
            _commandRegistry = commandRegistry;
        }

        public IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services)
        {
            services(_serviceCollection, _commandRegistry);
            return this;
        }

        public IFunctionHostBuilder Authorization(Action<IAuthorizationBuilder> authorization)
        {
            return this;
        }

        public IFunctionHostBuilder DefaultHttpHeaderBindingConfiguration(HeaderBindingConfiguration defaultConfiguration)
        {
            return this;
        }

        public IFunctionHostBuilder DefaultHttpResponseHandler<TResponseHandler>() where TResponseHandler : IHttpResponseHandler
        {
            return this;
        }

        public IFunctionHostBuilder AddValidator<TValidator>() where TValidator : IValidator
        {
            _serviceCollection.AddTransient(typeof(IValidator), typeof(TValidator));
            return this;
        }

        public IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions)
        {
            functions(_functionBuilder);
            return this;
        }

        public IFunctionHostBuilder OpenApiEndpoint(Action<IOpenApiBuilder> openApi)
        {
            return this;
        }

        public IFunctionHostBuilder OutputAuthoredSource(string folder)
        {
            return this;
        }

        public IFunctionHostBuilder ActionOnServiceProviderCreated(Action<IServiceProvider> action)
        {
            return this;
        }

        public IFunctionHostBuilder Serialization(Action<ISerializationBuilder> serialization)
        {
            return this;
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions => _functionBuilder.Definitions;
        public IAuthorizationBuilder AuthorizationBuilder { get; private set; }
        public ISerializationBuilder SerializationBuilder { get; }
        public Type ValidatorType { get; set; }
        public HeaderBindingConfiguration DefaultHeaderBindingConfiguration { get; }
        public Type DefaultHttpResponseHandlerType { get; }
    }
}
