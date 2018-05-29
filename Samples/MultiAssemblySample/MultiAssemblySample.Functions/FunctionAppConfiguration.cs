using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using MultiAssemblySample.Application;
using MultiAssemblySample.Commands;

namespace MultiAssemblySample.Functions
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) => { commandRegistry.AddApplication(); })
                .ProxiesEnabled(true)
                .OutputAuthoredSource("d:\\wip\\outputsource")
                .Functions(functions => functions
                    .HttpRoute("/api/v1/Simple", route => route
                        .HttpFunction<SimpleCommand>(AuthorizationTypeEnum.Anonymous, HttpMethod.Get)
                    )
                );
        }
    }
}
