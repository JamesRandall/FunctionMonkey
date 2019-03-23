using System.Net.Http;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
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
                        .AddValidatorsFromAssemblyContaining<FunctionAppConfiguration>()
                        ;
                })
                .AddFluentValidation()
                .OutputAuthoredSource(@"d:\wip\scratch\outputSource")
                .OpenApiEndpoint(openApi => openApi
                    .UserInterface()
                    .Title("Integration Test Functions")
                    .Version("1.0.0")
                )
                .Functions(functions => functions
                    // this is not really part of the test suite - but it needs to work - it sets up tables, containers, queues etc.
                    // essentially pre-reqs for tracking things in the test suite
                    .HttpRoute("setup", route => route
                        .HttpFunction<SetupTestResourcesCommand>()
                    )
                    .HttpRoute("verbs", route => route
                        .HttpFunction<HttpGetCommand>("/{value}", HttpMethod.Get)
                        .HttpFunction<HttpPostCommand>(HttpMethod.Post)
                        .HttpFunction<HttpPutCommand>(HttpMethod.Put)
                        .HttpFunction<HttpDeleteCommand>("/{value}", HttpMethod.Delete)
                        .HttpFunction<HttpPatchCommand>(new HttpMethod("PATCH"))
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
                    
                    
                    .HttpRoute("outputBindings", route => route
                        // Service Bus
                        .HttpFunction<HttpTriggerServiceBusQueueOutputCommand>("/toServiceBusQueue")
                        .OutputTo.ServiceBusQueue("serviceBusConnectionString", Constants.ServiceBus.MarkerQueue)

                        .HttpFunction<HttpTriggerServiceBusQueueCollectionOutputCommand>("/collectionToServiceBusQueue")
                        .OutputTo.ServiceBusQueue("serviceBusConnectionString", Constants.ServiceBus.MarkerQueue)

                        .HttpFunction<HttpTriggerServiceBusTopicOutputCommand>("/toServiceBusTopic")
                        .OutputTo.ServiceBusQueue("serviceBusConnectionString", Constants.ServiceBus.MarkerTopic)

                        .HttpFunction<HttpTriggerServiceBusTopicCollectionOutputCommand>("/collectionToServiceBusTopic")
                        .OutputTo.ServiceBusQueue("serviceBusConnectionString", Constants.ServiceBus.MarkerTopic)

                        // Storage

                        //.HttpFunction<HttpTriggerStorageBlobOutputCommandResultCommand>("/toBlobOutputWithName")
                        //.OutputTo.StorageBlob("storageConnectionString", "")

                        .HttpFunction<HttpTriggerStorageQueueOutputCommand>("/toStorageQueue")
                        .OutputTo.StorageQueue("storageConnectionString", Constants.Storage.Queue.MarkerQueue)

                        .HttpFunction<HttpTriggerStorageQueueCollectionOutputCommand>("/collectionToStorageQueue")
                        .OutputTo.StorageQueue("storageConnectionString", Constants.Storage.Queue.MarkerQueue)

                        .HttpFunction<HttpTriggerStorageTableOutputCommand>("/toStorageTable")
                        .OutputTo.StorageTable("storageConnectionString", Constants.Storage.Table.Markers)

                        .HttpFunction<HttpTriggerStorageTableCollectionOutputCommand>("/collectionToStorageTable")
                        .OutputTo.StorageTable("storageConnectionString", Constants.Storage.Table.Markers)

                        // Cosmos
                        .HttpFunction<HttpTriggerCosmosOutputCommand>("/toCosmos")
                        .OutputTo.Cosmos("cosmosConnectionString", Constants.Cosmos.Database, Constants.Cosmos.Collection)

                        .HttpFunction<HttpTriggerCosmosCollectionOutputCommand>("/collectionToCosmos")
                        .OutputTo.Cosmos("cosmosConnectionString", Constants.Cosmos.Database, Constants.Cosmos.Collection)                        
                    )                    
                    
                    
                    .Storage("storageConnectionString", storage => storage
                        .QueueFunction<StorageQueueCommand>(Constants.Storage.Queue.TestQueue)
                        .BlobFunction<BlobCommand>($"{Constants.Storage.Blob.BlobCommandContainer}/{{name}}")
                        .BlobFunction<StreamBlobCommand>(
                            $"{Constants.Storage.Blob.StreamBlobCommandContainer}/{{name}}")
                        .BlobFunction<BlogTriggerTableOutputCommand>($"{Constants.Storage.Blob.BlobCommandContainer}/{{name}}")

                        // This command isn't a direct test subject but it reads from a service bus queue and places
                        // the IDs into the marker table so that tests can find them during async output trigger testing
                        .QueueFunction<StorageQueuedMarkerIdCommand>(Constants.Storage.Queue.MarkerQueue)
                    )
                    .ServiceBus("serviceBusConnectionString", serviceBus => serviceBus
                        .QueueFunction<ServiceBusQueueCommand>(Constants.ServiceBus.Queue)
                        .SubscriptionFunction<ServiceBusSubscriptionCommand>(Constants.ServiceBus.TopicName,
                            Constants.ServiceBus.SubscriptionName)
                        .QueueFunction<ServiceBusQueueTriggerTableOutputCommand>(Constants.ServiceBus.TableOutputQueue)

                        // These commands aren't a direct test subject but read from a service bus queue and sub and places
                        // the IDs into the marker table so that tests can find them during async output trigger testing
                        .QueueFunction<ServiceBusQueuedMarkerIdCommand>(Constants.ServiceBus.MarkerQueue)
                        .SubscriptionFunction<ServiceBusSubscriptionMarkerIdCommand>(Constants.ServiceBus.MarkerTopic, Constants.ServiceBus.MarkerSubscription)
                    )
                    .CosmosDb("cosmosConnectionString", cosmos => cosmos
                        .ChangeFeedFunction<CosmosChangeFeedCommand>(Constants.Cosmos.Collection, Constants.Cosmos.Database, leaseCollectionPrefix:"l1")
                        .ChangeFeedFunction<CosmosTriggerTableOutputCommand>(Constants.Cosmos.Collection, Constants.Cosmos.Database, leaseCollectionPrefix: "l2")
                    )
                    .Timer<TimerCommand>("*/5 * * * * *")                    
                );
        }
    }
}
