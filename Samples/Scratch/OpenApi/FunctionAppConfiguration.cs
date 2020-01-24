using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using Microsoft.OpenApi.Models;
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
                    .InjectResource(Assembly.GetExecutingAssembly(), "Resources.OpenApi.app-logo-small.svg")
                    .InjectJavaScript(Assembly.GetExecutingAssembly(), "Resources.OpenApi.console-log.js")
                    .AddSecurityScheme("Bearer", // Name the security scheme
                        new OpenApiSecurityScheme
                        {
                            Description = "JWT Authorization header using the Bearer scheme.",
                            Type = SecuritySchemeType.Http, // We set the scheme type to http since we're using bearer authentication
                            Scheme = "bearer" // The name of the HTTP Authorization scheme to be used in the Authorization header. In this case "bearer".
                        })
                )
                .AddFluentValidation()
                .Functions(functions =>
                {
                    functions.RegisterCustomers();
                });
        }
    }
}
