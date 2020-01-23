using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using OpenApi.Customers;
using System.IO;
using System.Reflection;

namespace OpenApi
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .CompilerOptions(options => options
                    .OutputSourceTo(@"/Users/jamesrandall/code/authoredSource")
                )
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
                    .InjectStylesheet(Assembly.GetExecutingAssembly(), "Resources.OpenApi.theme-material.css")
                    .InjectStylesheet(Assembly.GetExecutingAssembly(), "Resources.OpenApi.custom.css")
                )
                .AddFluentValidation()
                .Functions(functions =>
                {
                    functions.RegisterCustomers();
                });
        }
    }
}
