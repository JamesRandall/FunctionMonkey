using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Builders
{
    public class FunctionHostBuilder : IFunctionHostBuilder
    {
        private readonly bool _isRuntime;
        public IServiceCollection ServiceCollection { get; }
        public ICommandRegistry CommandRegistry { get; }
        public IFunctionBuilder FunctionBuilder { get; } = new FunctionBuilder();
        public IAuthorizationBuilder AuthorizationBuilder { get; } = new AuthorizationBuilder();
        public Type ValidatorType { get; set; }
        public OpenApiConfiguration OpenApiConfiguration { get; set; } = new OpenApiConfiguration();
        public string OutputAuthoredSourceFolder { get; set; }
        public Action<IServiceProvider> ServiceProviderCreatedAction { get; set; }
        public bool AreProxiesEnabled { get; set; } = true;

        public FunctionHostBuilder(IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry, bool isRuntime)
        {
            _isRuntime = isRuntime;
            ServiceCollection = serviceCollection;
            CommandRegistry = commandRegistry;
        }

        public IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services)
        {
            if (_isRuntime)
            {
                services(ServiceCollection, CommandRegistry);
            }            
            return this;
        }

        public IFunctionHostBuilder Authorization(Action<IAuthorizationBuilder> authorization)
        {
            authorization(AuthorizationBuilder);
            return this;
        }

        public IFunctionHostBuilder AddValidator<TValidator>() where TValidator : IValidator
        {
            ValidatorType = typeof(TValidator);
            ServiceCollection.AddTransient(typeof(IValidator), ValidatorType);
            return this;
        }

        public IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions)
        {
            functions(FunctionBuilder);
            return this;
        }

        public IFunctionHostBuilder OpenApiEndpoint(Action<IOpenApiBuilder> openApi)
        {
            openApi(new OpenApiBuilder(OpenApiConfiguration));
            return this;
        }

        public IFunctionHostBuilder OutputAuthoredSource(string folder)
        {
            OutputAuthoredSourceFolder = folder;
            return this;
        }

        public IFunctionHostBuilder ProxiesEnabled(bool enabled)
        {
            AreProxiesEnabled = enabled;
            return this;
        }

        public IFunctionHostBuilder ActionOnServiceProviderCreated(Action<IServiceProvider> action)
        {
            ServiceProviderCreatedAction = action;
            return this;
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions => ((FunctionBuilder)FunctionBuilder).Definitions;
    }
}
