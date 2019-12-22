using System;
using System.Net.Http;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using SwaggerBuildOut.Commands;
using SwaggerBuildOut.Handlers;
using SwaggerBuildOut.Services;

namespace SwaggerBuildOut
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
                    commandRegistry.Register<HelloWorldCommandHandler>();
                    commandRegistry.Register<AddCommandHandler>();
                    commandRegistry.Register<CosmosCommandHandler>();
                    commandRegistry.Register<CosmosDocumentCommandHandler>();
                    commandRegistry.Register<CosmosDocumentBatchCommandHandler>();

                    serviceCollection.AddTransient<IMessageProvider, MessageProvider>();
                    
                    serviceCollection.AddLogging();
                })
                .OpenApiEndpoint(openApi => openApi
                    .Title("A Simple API")
                    .Version("0.0.0")
                    .UserInterface()
                )
                .Functions(functions => functions
                    .HttpRoute("/HelloWorld", route => route
                        .HttpFunction<HelloWorldCommand>(AuthorizationTypeEnum.Anonymous, HttpMethod.Post)    
                            .OpenApiDescription("Says hello world")
                            .Options(options => options
                                .AddHeaderMapping(x => x.HeaderName, "x-header-name")
                                .ResponseHandler<CustomResponseHandler>()
                            )
                    )
                   /*.HttpRoute("/Form", route => route
                        .HttpFunction<FormCommand>(HttpMethod.Post)
                    )
                    .OpenApiDescription("A route description")
                    //.CosmosDb("CosmosConnection", cosmos => cosmos
                        //.ChangeFeedFunction<CosmosCommand, ExampleCosmosErrorHandler>("Items", "ToDoList", leaseCollectionPrefix:"fn1")//, convertToPascalCase:true)
                        //.ChangeFeedFunction<CosmosDocumentCommand>("Items", "ToDoList")
                        //.ChangeFeedFunction<CosmosDocumentBatchCommand>("Items", "ToDoList", leaseCollectionPrefix:"fn2")
                    //)
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
                    /*.Storage("StorageConnectionString", storage => storage
                        .QueueFunction<HelloWorldCommand>("myqueue")
                    )
                .ServiceBus("ServiceBusConnectionString", sb => sb
                        .QueueFunction<HelloWorldCommand>("myqueue")    
                    .SubscriptionFunction<HelloWorldCommand>("mytopic", "mysub")
                    )*/
                );            
        }
    }
}
