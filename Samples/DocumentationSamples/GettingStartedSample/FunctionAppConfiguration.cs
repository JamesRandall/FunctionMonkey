using System.Net.Http;
using FluentValidation;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using GettingStartedSample.Commands;
using GettingStartedSample.Handlers;
using GettingStartedSample.Services;
using GettingStartedSample.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace GettingStartedSample
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    serviceCollection.AddTransient<IValidator<HelloWorldCommand>, HelloWorldCommandValidator>();
                    serviceCollection.AddTransient<IStringHasher, StringHasher>();
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })                
                .AddFluentValidation()
                .Functions(functions => functions
                    .HttpRoute("/api/v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>(HttpMethod.Get)
                    )
                );
        }
    }
}
