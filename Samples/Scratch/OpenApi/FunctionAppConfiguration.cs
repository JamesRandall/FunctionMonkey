using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using OpenApi.Customers;

namespace OpenApi
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Discover(typeof(FunctionAppConfiguration).Assembly);
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("My API Title")
                    .Version("1.2.3")
                    .UserInterface()
                )
                .Functions(functions =>
                {
                    functions.RegisterCustomers();
                });

        }
    }
}
