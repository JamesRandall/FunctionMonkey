using FluentValidation;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.FluentValidation;
using FunctionMonkey.Tests.Integration.Commands;
using FunctionMonkey.Tests.Integration.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace FunctionMonkey.Tests.Integration
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    serviceCollection
                        .AddTransient<IValidator<BlobCommand>, BlobCommandValidator>()
                        .AddTransient<IValidator<SimpleHttpRouteCommand>, SimpleHttpRouteCommandValidator>()
                        .AddTransient<IValidator<StorageQueueCommand>, StorageQueueCommandValidator>()
                    ;
                    commandRegistry.Discover<FunctionAppConfiguration>();
                })
                .AddFluentValidation()
                .OutputAuthoredSource(@"c:\wip\outputsource")
                .Functions(functions => functions
                    .HttpRoute("someroute", route => route
                        .HttpFunction<SimpleHttpRouteCommand>())
                    .Storage("storageaccount", storage => storage
                        .QueueFunction<StorageQueueCommand>("storagequeuecommandqueue")
                        .BlobFunction<BlobCommand>("blobcommandcontainer/{name}") // TODO: We need to have the compiler insert parameters on the function for everything surrounded in {} - e.g. {name} needs a string parameter of name
                        .BlobFunction<StreamBlobCommand>("streamblobcommandcontainer/{name}")
                    )
                    .CosmosDb("cosmosConnection", cosmos => cosmos
                        .ChangeFeedFunction<SimpleCosmosChangeFeedCommand>("cosmosCollection", "cosmosDatabase")
                    )
                    .ServiceBus("serviceBuConnection", serviceBus => serviceBus
                        .QueueFunction<SimpleServiceBusQueueCommand>("myQueue")
                        .SubscriptionFunction<SimpleServiceBusTopicCommand>("myTopic", "mySubscription")
                    )
                );
        }
    }
}
