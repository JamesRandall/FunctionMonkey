using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using SimpleRestApi.Queries;

namespace SimpleRestApi
{
    class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Discover<FunctionAppConfiguration>();
                })
                .OutputAuthoredSource("d:\\wip\\outputSource")
                .OpenApiEndpoint(openApi => openApi
                    .Title("Just A Test API")
                    .Servers("http://localhost:7001")
                    .Version("0.0.0")
                    .UserInterface()
                )
                .Functions(functions => functions
                    .HttpRoute("/api/v1/post", route => route
                        .HttpFunction<GetBlogPostQuery>(HttpMethod.Get)
                    )
                );
        }
    }
}
