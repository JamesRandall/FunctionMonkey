using System.Collections.Generic;
using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Compiler.Core;
using FunctionMonkey.FluentValidation;
using FunctionMonkey.Tests.Integration.AspNetCore;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.EventHubs;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.SignalR;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Common.Services;
using FunctionMonkey.Tests.Integration.Common.Validators;
using FunctionMonkey.TokenValidator;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Tests.Integration.Functions
{
    public class FunctionAppConfiguration2 : IFunctionAppConfiguration
    {
        // In the HTTP routes in addition to verbs we also need to cover the following variations of response
        // (it gets complicated due to permutations with the response handler and validation)
        //  1.  No response handler and a command with a result and no validation
        //  2.  No response handler and a command with a result and validation that passes
        //  3.  No response handler and a command with a result and validation that fails
        //  4.  No response handler and a command with no result and no validation
        //  5.  No response handler and a command with no result and validation that passes
        //  6.  No response handler and a command with no result and validation that fails
        //  7.  A response handler for a command with a result and no validation
        //  8.  A response handler for a command with no result and no validation
        //  9.  A response handler for a command with a result and a validator that passes - this should hit the CreateResponse method
        //  10. A response handler for a command with a result and a validator that fails - this should hit the CreateValidationFailureResponse method
        //  11. A response handler for a command with no result and a validator that passes - this should hit the CreateResponse method
        //  12. A response handler for a command with a result and a validator that fails - this should hit the CreateValidationFailureResponse method
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    serviceCollection
                        .AddTransient<IMarker, Marker>()
                        .AddValidatorsFromAssemblyContaining<HttpCommandWithNoResultAndValidatorThatFailsValidator>()
                        ;
                })
                .AddFluentValidation()
                .OpenApiEndpoint(openApi => openApi
                    .UserInterface()
                    .Title("Integration Test Functions")
                    .Version("1.0.0")
                )
                .Authorization(authorization => authorization
                    .AuthorizationDefault(AuthorizationTypeEnum.Anonymous)
                    .TokenValidator<MockTokenValidator>()
                    .Claims(claims => claims
                        .MapClaimToPropertyName("claima", "StringClaim")
                        .MapClaimToCommandProperty<HttpIntClaimCommand>("claimb", cmd => cmd.MappedValue)
                    )
                )
                .DefaultConnectionStringSettingNames(settingNames =>
                    {
                        // These are the default values - you don't have to set them
                        // I've set them here just to show what they are
                        settingNames.Storage = "storageConnectionString";
                        settingNames.CosmosDb = "cosmosConnectionString";
                        settingNames.ServiceBus = "serviceBusConnectionString";
                        settingNames.SignalR = "signalRConnectionString";
                    })
                .DefaultHttpHeaderBindingConfiguration(new HeaderBindingConfiguration()
                {
                    PropertyFromHeaderMappings = new Dictionary<string, string>
                    {
                        { "DefaultHeaderIntValue", "x-default-int" },
                        { "DefaultHeaderStringValue", "x-default-string" }
                    }
                })
                .Functions(functions => functions
                    // this is not really part of the test suite - but it needs to work - it sets up tables, containers, queues etc.
                    // essentially pre-reqs for tracking things in the test suite
                    .HttpRoute("setup", route => route
                        .HttpFunction<SetupTestResourcesCommand>(HttpMethod.Put)
                    )
                    .HttpRoute("claims", route => route
                        .HttpFunction<HttpStringClaimCommand>("/string", AuthorizationTypeEnum.TokenValidation, HttpMethod.Get)
                        .HttpFunction<HttpIntClaimCommand>("/int", AuthorizationTypeEnum.TokenValidation, HttpMethod.Get)
                    )
                    .HttpRoute("verbs", route => route
                        .HttpFunction<HttpGetCommand>("/{value}", HttpMethod.Get)
                        .HttpFunction<HttpPostCommand>(HttpMethod.Post)
                        .HttpFunction<HttpPostWithBytesCommand>("/bytes", HttpMethod.Post)
                        .HttpFunction<HttpPutCommand>(HttpMethod.Put)
                        .HttpFunction<HttpDeleteCommand>("/{value}", HttpMethod.Delete)
                        .HttpFunction<HttpPatchCommand>(new HttpMethod("PATCH"))
                    )
                    .HttpRoute("securityProperty", route => route
                        .HttpFunction<HttpPostCommandWithSecurityProperty>(HttpMethod.Post)
                        .HttpFunction<HttpGetCommandWithSecurityProperty>(HttpMethod.Get)
                    )
                    .HttpRoute("withLogger", route => route
                        .HttpFunction<HttpGetWithLoggerCommand>(HttpMethod.Get)
                    )
                    .HttpRoute("queryParameters", route => route
                        .HttpFunction<HttpGetQueryParamCommand>(HttpMethod.Get)
                        .HttpFunction<HttpGetGuidQueryParameterCommand>("/guidQueryParam", HttpMethod.Get)
                        .HttpFunction<HttpArrayQueryParamCommand>("/array", HttpMethod.Get)
                        .HttpFunction<HttpIReadOnlyCollectionQueryParamCommand>("/readonlyCollection", HttpMethod.Get)
                        .HttpFunction<HttpListQueryParamCommand>("/list", HttpMethod.Get)
                        .HttpFunction<HttpListQueryStringParamCommand>("/stringList", HttpMethod.Get)
                        .HttpFunction<HttpIEnumerableQueryParamCommand>("/enumerable", HttpMethod.Get)
                    )
                    .HttpRoute("routeParameters", route => route
                        .HttpFunction<HttpGetRouteParameterCommand>("/{message}/{value:int}/{optionalValue?}/{optionalMessage?}")
                        .HttpFunction<HttpGetGuidRouteParameterCommand>("/guids/{requiredGuid}/{optionalGuid?}")
                    )
                    .HttpRoute("noResponseHandler", route => route
                        // These are the functions for testing the HTTP route cases outlined above
                        .HttpFunction<HttpCommandWithNoResultAndNoValidation>("/noResult/noValidation")
                        .HttpFunction<HttpCommandWithNoResultAndValidatorThatFails>("/noResult/validationFails")
                        .HttpFunction<HttpCommandWithNoResultAndValidatorThatPasses>("/noResult/validationPasses")
                        .HttpFunction<HttpCommandWithResultAndNoValidation>("/result/noValidation")
                        .HttpFunction<HttpCommandWithResultAndValidatorThatFails>("/result/validationFails")
                        .HttpFunction<HttpCommandWithResultAndValidatorThatPasses>("/result/validationPasses")
                    )
                    .HttpRoute("headers", route => route
                        .HttpFunction<HttpHeaderBindingCommand>()
                        .Options(options => options
                            .AddHeaderMapping(cmd => cmd.Value, "x-value")
                            .AddHeaderMapping(cmd => cmd.Message, "x-message")
                        )
                        .HttpFunction<HttpHeaderNullableValueTypeBindingCommand>("/nullableValueType")
                        .Options(options => options
                            .AddHeaderMapping(cmd => cmd.Value, "x-value")
                        )
                        .HttpFunction<HttpHeaderEnumTypeBindingCommand>("/enumType")
                        .Options(options => options
                            .AddHeaderMapping(cmd => cmd.Value, "x-enum-value")
                        )
                        .HttpFunction<HttpDefaultHeaderCommand>("/default")
                    )
                    .HttpRoute("responseHandler", route => route
                        .HttpFunction<HttpResponseHandlerCommandWithNoResultAndNoValidation>(
                            "/noResult/noValidation")
                        .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<HttpResponseHandlerCommandWithNoResultAndValidatorThatFails>(
                            "/noResult/validationFails")
                        .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>(
                            "/noResult/validationPasses")
                        .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<HttpResponseHandlerCommandWithResultAndNoValidation>("/result/noValidation")
                        .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<HttpResponseHandlerCommandWithResultAndValidatorThatFails>(
                            "/result/validationFails")
                        .Options(options => options.ResponseHandler<CustomResponseHandler>())
                        .HttpFunction<HttpResponseHandlerCommandWithResultAndValidatorThatPasses>(
                            "/result/validationPasses")
                        .Options(options => options.ResponseHandler<CustomResponseHandler>())
                    )

                    .HttpRoute(route => route
                        .HttpFunction<HttpCommandWithNoRoute>()
                    )
                    
                    /*
                    // Output bindings
                    .HttpRoute("outputBindings", route => route
                        // Service Bus
                        .HttpFunction<HttpTriggerServiceBusQueueOutputCommand>("/toServiceBusQueue")
                        .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerQueue)

                        .HttpFunction<HttpTriggerServiceBusQueueCollectionOutputCommand>("/collectionToServiceBusQueue")
                        .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerQueue)

                        .HttpFunction<HttpTriggerServiceBusTopicOutputCommand>("/toServiceBusTopic")
                        .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerTopic)

                        .HttpFunction<HttpTriggerServiceBusTopicCollectionOutputCommand>("/collectionToServiceBusTopic")
                        .OutputTo.ServiceBusTopic(Constants.ServiceBus.MarkerTopic)
                        
                        .HttpFunction<HttpTriggerServiceBusQueueWithSessionIdOutputCommand>("/toServiceBusQueueWithSessionId")
                        .Options(options => options.NoCommandHandler())
                        .OutputTo.ServiceBusQueue<HttpTriggerServiceBusTopicWithSessionIdOutputCommand>(
                            Constants.ServiceBus.MarkerQueueWithSessionId, 
                            command => command.ASessionId)
                        
                        .HttpFunction<HttpTriggerServiceBusTopicWithSessionIdOutputCommand>("/toServiceBusTopicWithSessionId")
                        .OutputTo.ServiceBusTopic<HttpTriggerServiceBusTopicWithSessionIdOutputCommand>(
                            Constants.ServiceBus.MarkerTopicWithSessionId, 
                            command => command.ASessionId)
                        
                        .HttpFunction<HttpTriggerServiceBusQueueWithSessionIdResultOutputCommand>("/toServiceBusQueueWithResultSessionId")
                        .OutputTo.ServiceBusQueue<ModelWithSessionId>(
                            Constants.ServiceBus.MarkerQueueWithSessionId, 
                            result => result.SessionId)
                        
                        // Event hubs
                        .HttpFunction<HttpTriggerEventHubOutputCommand>("/toEventHub")
                        .OutputTo.EventHub(Constants.EventHub.OutputHub)

                        // Storage

                        //.HttpFunction<HttpTriggerStorageBlobOutputCommandResultCommand>("/toBlobOutputWithName")
                        //.OutputTo.StorageBlob("storageConnectionString", "")

                        .HttpFunction<HttpTriggerStorageQueueOutputCommand>("/toStorageQueue")
                        .OutputTo.StorageQueue(Constants.Storage.Queue.MarkerQueue)

                        .HttpFunction<HttpTriggerStorageQueueCollectionOutputCommand>("/collectionToStorageQueue")
                        .OutputTo.StorageQueue(Constants.Storage.Queue.MarkerQueue)

                        .HttpFunction<HttpTriggerStorageTableOutputCommand>("/toStorageTable")
                        .OutputTo.StorageTable(Constants.Storage.Table.Markers)

                        .HttpFunction<HttpTriggerStorageTableCollectionOutputCommand>("/collectionToStorageTable")
                        .OutputTo.StorageTable(Constants.Storage.Table.Markers)

                        // Cosmos
                        .HttpFunction<HttpTriggerCosmosOutputCommand>("/toCosmos")
                        .OutputTo.CosmosDb(Constants.Cosmos.Collection, Constants.Cosmos.Database)

                        .HttpFunction<HttpTriggerCosmosCollectionOutputCommand>("/collectionToCosmos")
                        .OutputTo.CosmosDb(Constants.Cosmos.Collection, Constants.Cosmos.Database)
                    )
                    */
                );
        }
    }
}
