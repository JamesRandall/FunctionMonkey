using System;
using System.Collections.Generic;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
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

                    serviceCollection.AddLogging();
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("A Simple API")
                    .Version("0.0.0")
                    .UserInterface()
                )
                .OutputAuthoredSource(@"c:\wip\scratch\outputSource")
                .Functions(functions => functions
                    .HttpRoute("/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>("/{name}", AuthorizationTypeEnum.Anonymous, HttpMethod.Get)    
                            .OpenApiDescription("Says hello world")
                            .Options(options => options
                                .AddHeaderMapping(x => x.HeaderName, "x-header-name")
                                .ResponseHandler<CustomResponseHandler>()
                            )
                    )
                   .HttpRoute("/Form", route => route
                        .HttpFunction<FormCommand>(HttpMethod.Post)
                    )
                    .OpenApiDescription("A route description")
                    .CosmosDb("CosmosConnection", cosmos => cosmos
                        .ChangeFeedFunction<CosmosCommand, ExampleCosmosErrorHandler>("Items", "ToDoList", leaseCollectionPrefix:"fn1")//, convertToPascalCase:true)
                        //.ChangeFeedFunction<CosmosDocumentCommand>("Items", "ToDoList")
                        //.ChangeFeedFunction<CosmosDocumentBatchCommand>("Items", "ToDoList", leaseCollectionPrefix:"fn2")
                    )
                    .HttpRoute("/Add", route => route
                        .HttpFunction<AddCommand>(AuthorizationTypeEnum.Anonymous,HttpMethod.Post)
                        .OpenApiDescription("Adds two numbers together")
                        .OpenApiResponse(400, "Some sort of error")
                        //.Serializer<DefaultCaseJsonSerializer>()
                        .Options(options => options    
                            .JsonNamingStrategies<DefaultNamingStrategy, SnakeCaseNamingStrategy>()
                        )
                    )
                    /*.OpenApiName("HelloWorld")*/
                    //.Timer<HelloWorldCommand, HelloWorldTimerCommandFactory>("*/5 * * * * *")
                    .Storage("StorageConnectionString", storage => storage
                        .QueueFunction<HelloWorldCommand>("myqueue")
                    )
                .ServiceBus("ServiceBusConnectionString", sb => sb
                        .QueueFunction<HelloWorldCommand>("myqueue")    
                    .SubscriptionFunction<HelloWorldCommand>("mytopic", "mysub")
                    )
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
