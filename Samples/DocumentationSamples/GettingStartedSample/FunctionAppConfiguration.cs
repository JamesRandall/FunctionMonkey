using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using GettingStartedSample.Commands;
using GettingStartedSample.Handlers;
using GettingStartedSample.Services;
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
                    serviceCollection.AddTransient<IStringHasher, StringHasher>();
                    commandRegistry.Register<HelloWorldCommandHandler>();
                })
                .Functions(functions => functions
                    .HttpRoute("/api/v1/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>()
                    )
                );
        }
    }
}
