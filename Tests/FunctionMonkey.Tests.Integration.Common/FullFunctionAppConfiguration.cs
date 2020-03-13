using System.Net.Http;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.EventHubs;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.SignalR;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class FullFunctionAppConfiguration : FunctionAppConfigurationBase
    {
        protected override IFunctionBuilder CreateAdditionalFunctions(IFunctionBuilder builder)
        {
            builder
                .HttpRoute("outputBindings", route => route
                    // Service Bus
                    .HttpFunction<HttpTriggerServiceBusQueueOutputCommand>("/toServiceBusQueue")
                    .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerQueue)
                    
                    .HttpFunction<HttpTriggerServiceBusQueueOutputWithConverterCommand>("/toServiceBusQueueWithConverter")
                    .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerQueue)
                    .OutputBindingConverter<OptionalValueCommandOutputBindingConverter>()

                    .HttpFunction<HttpTriggerServiceBusQueueCollectionOutputCommand>("/collectionToServiceBusQueue")
                    .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerQueue)

                    .HttpFunction<HttpTriggerServiceBusTopicOutputCommand>("/toServiceBusTopic")
                    .OutputTo.ServiceBusQueue(Constants.ServiceBus.MarkerTopic)

                    .HttpFunction<HttpTriggerServiceBusTopicCollectionOutputCommand>("/collectionToServiceBusTopic")
                    .OutputTo.ServiceBusTopic(Constants.ServiceBus.MarkerTopic)

                    .HttpFunction<HttpTriggerServiceBusQueueWithSessionIdOutputCommand>(
                        "/toServiceBusQueueWithSessionId")
                    .Options(options => options.NoCommandHandler())
                    .OutputTo.ServiceBusQueue<HttpTriggerServiceBusTopicWithSessionIdOutputCommand>(
                        Constants.ServiceBus.MarkerQueueWithSessionId,
                        command => command.ASessionId)

                    .HttpFunction<HttpTriggerServiceBusTopicWithSessionIdOutputCommand>(
                        "/toServiceBusTopicWithSessionId")
                    .OutputTo.ServiceBusTopic<HttpTriggerServiceBusTopicWithSessionIdOutputCommand>(
                        Constants.ServiceBus.MarkerTopicWithSessionId,
                        command => command.ASessionId)

                    .HttpFunction<HttpTriggerServiceBusQueueWithSessionIdResultOutputCommand>(
                        "/toServiceBusQueueWithResultSessionId")
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

                // SignalR tests
                .HttpRoute("signalR", route => route
                    .HttpFunction<SendMessageCommand>("/messageToAll")
                    .OutputTo.SignalRMessage(Constants.SignalR.HubName)

                    .HttpFunction<SendMessageToGroupCommand>("/messageToGroup/{groupName}")
                    .OutputTo.SignalRMessage(Constants.SignalR.HubName)

                    .HttpFunction<SendMessageCollectionCommand>("/messageCollectionToUser", HttpMethod.Post)
                    .OutputTo.SignalRMessage(Constants.SignalR.HubName)

                    .HttpFunction<AddUserToGroupCommand>("/addUserToGroup", HttpMethod.Put)
                    .OutputTo.SignalRGroupAction(Constants.SignalR.HubName)

                    .HttpFunction<RemoveUserFromGroupCommand>("/removeUserFromGroup", HttpMethod.Put)
                    .OutputTo.SignalRGroupAction(Constants.SignalR.HubName)
                )
                .ServiceBus(serviceBus => serviceBus
                    .QueueFunction<SendMessageToUserCommand>(Constants.ServiceBus.SignalRQueue)
                    .OutputTo.SignalRMessage(Constants.SignalR.HubName)
                )
                .SignalR(signalR => signalR
                    .Negotiate<NegotiateCommand>("/negotiate")
                    .Negotiate("/simpleNegotiate", Constants.SignalR.HubName, "{headers.x-ms-client-principal-id}")
                )

                // Storage
                .Storage(storage => storage
                    .QueueFunction<StorageQueueCommand>(Constants.Storage.Queue.TestQueue)
                    .BlobFunction<BlobCommand>($"{Constants.Storage.Blob.BlobCommandContainer}/{{name}}")
                    .BlobFunction<StreamBlobCommand>(
                        $"{Constants.Storage.Blob.StreamBlobCommandContainer}/{{name}}")
                    .BlobFunction<BlogTriggerTableOutputCommand>(
                        $"{Constants.Storage.Blob.BlobOutputCommandContainer}/{{name}}")
                    .OutputTo.StorageTable(Constants.Storage.Table.Markers)

                    // This command isn't a direct test subject but it reads from a service bus queue and places
                    // the IDs into the marker table so that tests can find them during async output trigger testing
                    .QueueFunction<StorageQueuedMarkerIdCommand>(Constants.Storage.Queue.MarkerQueue)
                )

                .ServiceBus(serviceBus => serviceBus
                    .QueueFunction<ServiceBusQueueCommand>(Constants.ServiceBus.Queue)
                    .SubscriptionFunction<ServiceBusSubscriptionCommand>(Constants.ServiceBus.TopicName,
                        Constants.ServiceBus.SubscriptionName)
                    .QueueFunction<ServiceBusQueueTriggerTableOutputCommand>(Constants.ServiceBus.TableOutputQueue)
                    .OutputTo.StorageTable(Constants.Storage.Table.Markers)

                    .QueueFunction<ServiceBusSessionIdQueueCommand>(Constants.ServiceBus.SessionIdQueue, true)
                    .SubscriptionFunction<ServiceBusSessionIdSubscriptionCommand>(
                        Constants.ServiceBus.SessionIdTopicName,
                        Constants.ServiceBus.SessionIdSubscriptionName, true)

                    // These commands aren't a direct test subject but read from a service bus queue and sub and places
                    // the IDs into the marker table so that tests can find them during async output trigger testing
                    .QueueFunction<ServiceBusQueuedMarkerIdCommand>(Constants.ServiceBus.MarkerQueue)
                    .SubscriptionFunction<ServiceBusSubscriptionMarkerIdCommand>(Constants.ServiceBus.MarkerTopic,
                        Constants.ServiceBus.MarkerSubscription)
                )

                .CosmosDb(cosmos => cosmos
                    .ChangeFeedFunction<CosmosChangeFeedCommand>(Constants.Cosmos.Collection, Constants.Cosmos.Database)
                    .ChangeFeedFunction<CosmosTriggerTableOutputCommand>(Constants.Cosmos.OutputTableCollection,
                        Constants.Cosmos.Database, leaseCollectionName: Constants.Cosmos.OutputTableLeases)
                    .OutputTo.StorageTable(Constants.Storage.Table.Markers)
                )

                .EventHub(eventHub => eventHub
                    .EventHubFunction<EventHubCommand>(Constants.EventHub.HubName)
                    // These commands aren't a direct test subject but read from a service bus queue and sub and places
                    // the IDs into the marker table so that tests can find them during async output trigger testing
                    .EventHubFunction<EventHubQueuedMarkerIdCommand>(Constants.EventHub.OutputHub)
                )

                .Timer<TimerCommand>("*/5 * * * * *");
            return builder;
        }
    }
}