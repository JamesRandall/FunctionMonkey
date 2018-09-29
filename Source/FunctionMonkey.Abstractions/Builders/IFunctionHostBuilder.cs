using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IFunctionHostBuilder
    {
        /// <summary>
        /// Surfaces an IServiceCollection into which dependencies (for command handlers) can be registered
        /// </summary>
        /// <param name="services">An action that will be given a command registry and service collection</param>
        /// <returns>The function host builder for use in a Fluent API</returns>
        IFunctionHostBuilder Setup(Action<IServiceCollection, ICommandRegistry> services);

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
        /// Registers a validator with the Functions runtime
        /// </summary>
        /// <typeparam name="TValidator">A validator implementation, must implement the IValidator interface</typeparam>
        /// <returns></returns>
        IFunctionHostBuilder AddValidator<TValidator>() where TValidator : IValidator;

        /// <summary>
        /// Surfaces a builder for declaring the command to function bindings
        /// </summary>
        /// <param name="functions">The function builder</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder Functions(Action<IFunctionBuilder> functions);

        /// <summary>
        /// Surfaces a builder that allows Open API to be configured. If this builder
        /// is not used then no document is included.
        /// </summary>
        /// <param name="openApi">Open API builder</param>
        /// <returns>The function host builder to support a Fluent API</returns>
        IFunctionHostBuilder OpenApiEndpoint(Action<IOpenApiBuilder> openApi);

        /// <summary>
        /// A diagnostic option to output the source that has been authored for the Function triggers
        /// </summary>
        /// <param name="folder">The folder to output to</param>
        IFunctionHostBuilder OutputAuthoredSource(string folder);

        /// <summary>
        /// Allows HTTP proxy generation to be enabled (true) or disabled (false)
        /// </summary>
        IFunctionHostBuilder ProxiesEnabled(bool enabled);

        /// <summary>
        /// When a service provider is created the action supplied here will be invoked
        /// </summary>
        /// <param name="action">An action to invoke when a service provider is created</param>
        IFunctionHostBuilder ActionOnServiceProviderCreated(Action<IServiceProvider> action);
    }
}
