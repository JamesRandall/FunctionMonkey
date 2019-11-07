using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
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
                    serviceCollection.AddValidatorsFromAssembly(typeof(FunctionAppConfiguration).Assembly);
                    commandRegistry.Discover(typeof(FunctionAppConfiguration).Assembly);
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("My API Title 2.0.0-beta-113")
                    .Version("v2")
                    .UserInterface()
                    .AddValidatorsFromAssembly(typeof(FunctionAppConfiguration).Assembly)
                    .AddXmlComments(Path.Combine(Path.GetDirectoryName(typeof(FunctionAppConfiguration).Assembly.Location), "OpenApi.xml"))
                    .CustomSchemaIds()
                )
                .AddFluentValidation()
                .Functions(functions =>
                {
                    functions.RegisterCustomers();
                });

        }
    }
}
