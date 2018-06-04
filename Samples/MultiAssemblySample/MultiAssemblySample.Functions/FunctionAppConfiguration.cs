using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;
using MultiAssemblySample.Application;
using MultiAssemblySample.Commands;

namespace MultiAssemblySample.Functions
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    serviceCollection.AddLogging();
                    commandRegistry.AddApplication();
                })
                .ProxiesEnabled(true)
                .OutputAuthoredSource("d:\\wip\\outputsource")
                .Functions(functions => functions
                    .HttpRoute("/api/v1/Simple", route => route
                        .HttpFunction<SimpleCommand>(AuthorizationTypeEnum.Anonymous, HttpMethod.Get)
                        .HttpFunction<SimpleCommandWithResult>(AuthorizationTypeEnum.Anonymous, HttpMethod.Post)
                        .HttpFunction<SimpleCommandWithValidationResult>(AuthorizationTypeEnum.Anonymous, HttpMethod.Put)
                    )
                );
        }
    }
}
