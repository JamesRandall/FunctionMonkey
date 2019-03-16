using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Testing
{
    public class TestFunctionHostBuilder : IFunctionHostBuilder
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly ICommandRegistry _commandRegistry;

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
    }
}
