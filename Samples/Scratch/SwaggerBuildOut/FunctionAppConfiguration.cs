using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
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
                    commandRegistry.Register<CosmosCommandHandler>();
                    commandRegistry.Register<CosmosDocumentCommandHandler>();
                    commandRegistry.Register<CosmosDocumentBatchCommandHandler>();
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("A Simple API")
                    .Version("0.0.0")
                    .UserInterface()
                )
                .OutputAuthoredSource(@"d:\wip\scratch\outputSource")
                .Functions(functions => functions
                    .HttpRoute("/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>("/{name}", AuthorizationTypeEnum.Anonymous, HttpMethod.Get)    
                            .OpenApiDescription("Says hello world")
                            .AddHeaderMapping(x => x.HeaderName, "x-header-name")
                            //.ResponseHandler<CustomResponseHandler>()
                    )
                    .OpenApiDescription("A route description")
                    .CosmosDb("CosmosConnection", cosmos => cosmos
                        .ChangeFeedFunction<CosmosCommand>("Items", "ToDoList", leaseCollectionPrefix:"fn1")//, convertToPascalCase:true)
                        //.ChangeFeedFunction<CosmosDocumentCommand>("Items", "ToDoList")
                        .ChangeFeedFunction<CosmosDocumentBatchCommand>("Items", "ToDoList", leaseCollectionPrefix:"fn2")
                    )
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
