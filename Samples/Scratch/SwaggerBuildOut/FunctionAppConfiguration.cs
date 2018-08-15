using System;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;
using SwaggerBuildOut.Commands;
using SwaggerBuildOut.Handlers;

namespace SwaggerBuildOut
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration, IContainerProvider
    {
        public void Build(IFunctionHostBuilder builder)
        {
            
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Register<HelloWorldCommandHandler>();
                    commandRegistry.Register<AddCommandHandler>();
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("A Simple API")
                    .Version("0.0.0")
                    .UserInterface()
                )
                .OutputAuthoredSource(@"d:\wip\outputSource")
                .Functions(functions => functions
                    .HttpRoute("/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>("/{name}", HttpMethod.Get)    
                        .OpenApiDescription("Says hello world")
                    )
                    .OpenApiDescription("A route description")
                    /*.HttpRoute("/Add", route => route
                        .HttpFunction<AddCommand>(AuthorizationTypeEnum.Anonymous,HttpMethod.Post)
                        .OpenApiDescription("Adds two numbers together")
                        .OpenApiResponse(400, "Some sort of error")
                    )
                    .OpenApiName("HelloWorld")*/
                    //.Timer<HelloWorldCommand, HelloWorldTimerCommandFactory>("*/5 * * * * *")
                    //.Storage("StorageConnectionString", storage => storage
                        //.BlobFunction<HelloWorldCommand>("triggertest/{name}"))
                );            
        }

        public IServiceCollection CreateServiceCollection()
        {
            return new ServiceCollection();
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection serviceCollection)
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            IContainer autofacContainer = containerBuilder.Build();
            IServiceProvider serviceProvider = new AutofacServiceProvider(autofacContainer);
            return serviceProvider;
        }
    }
}
