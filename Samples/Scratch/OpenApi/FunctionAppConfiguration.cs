using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using OpenApi.Customers;
using System.IO;

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
                    .Version("v2")
                    .UserInterface()
                    .IncludeXmlComments(Path.Combine(Path.GetDirectoryName(typeof(FunctionAppConfiguration).Assembly.Location), "OpenApi.xml"))
                )
                .Functions(functions =>
                {
                    functions.RegisterCustomers();
                });

        }
    }
}
