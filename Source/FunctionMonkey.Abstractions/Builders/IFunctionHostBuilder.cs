using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IFunctionHostBuilder
    {
        /// <summary>
        /// Surfaces an IServiceCollection into which dependencies (can be registered
        /// </summary>
        /// <param name="services">An action that will be given a service collection</param>
        /// <returns>The function host builder for use in a Fluent API</returns>
        IFunctionHostBuilder Setup(Action<IServiceCollection> services);
        
        /// <summary>
        /// Surfaces an IServiceCollection into which dependencies (for command handlers) can be registered
        /// </summary>
        /// <param name="services">An action that will be given a command registry and service collection</param>
        /// <returns>The function host builder for use in a Fluent API</returns>
        IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services);

        /// <summary>
        /// Allows a mediator to be registered
        /// </summary>
        /// <typeparam name="TMediator">The concrete type of the mediator</typeparam>
        /// <returns>The function host builder for use in a Fluent API</returns>
        IFunctionHostBuilder Mediator<TMediator>() where TMediator : IMediatorDecorator;

        /// <summary>
        /// Allows the default setting names to be specified - see ConnectionStringSettingNames for the defaults.
        /// Currently this must be called before Functions - a future API breaking change will not require this.
        /// </summary>
        /// <param name="settingNames">The settings, new values can be set</param>
        /// <returns></returns>
        IFunctionHostBuilder DefaultConnectionStringSettingNames(Action<ConnectionStringSettingNames> settingNames);

        /// <summary>
        /// Surfaces a builder for configurating authorization
        /// </summary>
        /// <param name="authorization"></param>
        /// <returns></returns>
        IFunctionHostBuilder Authorization(Action<IAuthorizationBuilder> authorization);

        /// <summary>
        /// Sets the default HTTP header binding configuration. The default mode is no binding.
        /// </summary>
        /// <param name="defaultConfiguration"></param>
        /// <returns></returns>
        IFunctionHostBuilder DefaultHttpHeaderBindingConfiguration(HeaderBindingConfiguration defaultConfiguration);

        /// <summary>
        /// Allows a response handler to be registered for all HTTP requests. See IHttpResponseHandler documentation
        /// for details of how this functions.
        ///
        /// By default no response handler is configured and all responses will be controlled by the framework.
        /// </summary>
        /// <typeparam name="TResponseHandler">A type of IHttpResponseHandler</typeparam>
        /// <returns></returns>
        IFunctionHostBuilder DefaultHttpResponseHandler<TResponseHandler>() where TResponseHandler : IHttpResponseHandler;

        /// <summary>
        /// Registers a validator with the Functions runtime
        /// </summary>
        /// <typeparam name="TValidator">A validator implementation, must implement the IValidator interface</typeparam>
        /// <returns></returns>
        IFunctionHostBuilder AddValidator<TValidator>() where TValidator : IValidator;

        /// <summary>
        /// Surfaces a builder for declaring the command to function bindings
        /// The functions declaration needs to be the last thing that is configured so it doesn't return a builder
        /// </summary>
        /// <param name="functions">The function builder</param>
        void Functions(Action<IFunctionBuilder> functions);

        /// <summary>
        /// Surfaces a builder that allows Open API to be configured. If this builder
        /// is not used then no document is included.
        /// </summary>
        /// <param name="openApi">Open API builder</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder OpenApiEndpoint(Action<IOpenApiBuilder> openApi);

        /// <summary>
        /// Allows default for serialization to be configured
        /// </summary>
        /// <param name="serialization">A serialization builder</param>
        IFunctionHostBuilder Serialization(Action<ISerializationBuilder> serialization);

        /// <summary>
        /// Compiler options such as output target
        /// </summary>
        IFunctionHostBuilder CompilerOptions(Action<ICompilerOptionsBuilder> options);

        /// <summary>
        /// Registers a default output converter to use for all triggers with an output parameter
        /// </summary>
        /// <typeparam name="TConverter"></typeparam>
        /// <returns></returns>
        IFunctionHostBuilder DefaultOutputConverter<TConverter>() where TConverter : IOutputBindingConverter;
        
        /// <summary>
        /// Marks all functions as having no command handlers - this can be useful if building a set of functions
        /// that purely route inputs to output bindings
        /// </summary>
        /// <returns></returns>
        IFunctionHostBuilder NoCommandHandlers();
        
        /// <summary>
        /// Allows a transformer to be registered that will apply to all commands unless respecified on a function
        IFunctionHostBuilder DefaultCommandTransformer<TCommandTransformer>() where TCommandTransformer : ICommandTransformer;

        /// <summary>
        /// Access to the registered function definitions
        /// </summary>
        IReadOnlyCollection<AbstractFunctionDefinition> FunctionDefinitions { get; }
    }
}
