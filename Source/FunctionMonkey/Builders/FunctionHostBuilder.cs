using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Abstractions.Validation;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Model;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Builders
{
    /// <exclude />
    public class FunctionHostBuilder : IFunctionHostBuilder
    {
        private readonly bool _isRuntime;
        public IServiceCollection ServiceCollection { get; }
        public ICommandRegistry CommandRegistry { get; }
        public IFunctionBuilder FunctionBuilder { get; }
        public IAuthorizationBuilder AuthorizationBuilder { get; } = new AuthorizationBuilder();
        public Type ValidatorType { get; set; }
        public OpenApiConfiguration OpenApiConfiguration { get; } = new OpenApiConfiguration();
        public HeaderBindingConfiguration DefaultHeaderBindingConfiguration { get; private set; }
        public Type DefaultHttpResponseHandlerType { get; private set; }
        public ISerializationBuilder SerializationBuilder { get; } = new SerializationBuilder();
        public ConnectionStringSettingNames ConnectionStringSettingNames { get; } = new ConnectionStringSettingNames();
        public CompilerOptions Options { get; set; } = new CompilerOptions();
        public Type MediatorType { get; set; } = typeof(DefaultMediatorDecorator);
        public Type DefaultOutputBindingConverter { get; set; }
        public bool HasNoCommandHandlers { get; set; }
        
        public Type DefaultGlobalCommandTypeTransformer { get; set; }
        
        public FunctionHostBuilder(IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry, bool isRuntime)
        {
            _isRuntime = isRuntime;
            ServiceCollection = serviceCollection;
            CommandRegistry = commandRegistry;
            FunctionBuilder = new FunctionBuilder(ConnectionStringSettingNames);
        }

        public IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services)
        {
            if (_isRuntime)
            {
                services(ServiceCollection, CommandRegistry);
            }
            return this;
        }
        
        public IFunctionHostBuilder Setup(Action<IServiceCollection> services)
        {
            if (_isRuntime)
            {
                services(ServiceCollection);
            }
            return this;
        }

        public IFunctionHostBuilder Mediator<TMediator>() where TMediator : IMediatorDecorator
        {
            MediatorType = typeof(TMediator);
            return this;
        }

        public IFunctionHostBuilder DefaultConnectionStringSettingNames(Action<ConnectionStringSettingNames> settingNames)
        {
            settingNames(ConnectionStringSettingNames);
            return this;
        }

        public IFunctionHostBuilder Authorization(Action<IAuthorizationBuilder> authorization)
        {
            authorization(AuthorizationBuilder);
            return this;
        }

        public IFunctionHostBuilder DefaultHttpHeaderBindingConfiguration(HeaderBindingConfiguration defaultConfiguration)
        {
            DefaultHeaderBindingConfiguration = defaultConfiguration;
            return this;
        }

        public IFunctionHostBuilder DefaultHttpResponseHandler<TResponseHandler>()
            where TResponseHandler : IHttpResponseHandler
        {
            DefaultHttpResponseHandlerType = typeof(TResponseHandler);
            return this;
        }

        public IFunctionHostBuilder AddValidator<TValidator>() where TValidator : IValidator
        {
            ValidatorType = typeof(TValidator);
            ServiceCollection.AddTransient(typeof(IValidator), ValidatorType);
            return this;
        }

        public void Functions(Action<IFunctionBuilder> functions)
        {
            functions(FunctionBuilder);
        }

        public IFunctionHostBuilder OpenApiEndpoint(Action<IOpenApiBuilder> openApi)
        {
            openApi(new OpenApiBuilder(OpenApiConfiguration));
            return this;
        }

        public IFunctionHostBuilder Serialization(Action<ISerializationBuilder> serialization)
        {
            serialization(SerializationBuilder);
            return this;
        }

        public IFunctionHostBuilder CompilerOptions(Action<ICompilerOptionsBuilder> options)
        {
            options(new CompilerOptionsBuilder(Options));
            return this;
        }

        public IFunctionHostBuilder DefaultOutputConverter<TConverter>() where TConverter : IOutputBindingConverter
        {
            DefaultOutputBindingConverter = typeof(TConverter);
            return this;
        }

        public IFunctionHostBuilder NoCommandHandlers()
        {
            HasNoCommandHandlers = true;
            return this;
        }

        public IFunctionHostBuilder DefaultCommandTransformer<TCommandTransformer>() where TCommandTransformer : ICommandTransformer
        {
            DefaultGlobalCommandTypeTransformer = typeof(TCommandTransformer);
            return this;
        }

        public IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions => ((FunctionBuilder)FunctionBuilder).Definitions;
    }
}
