using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                {
                    commandRegistry.Discover<FunctionAppConfiguration>();
                })
                .OutputAuthoredSource(@"c:\wip\outputsource")
                .Functions(functions => functions
                    //.HttpRoute("someroute", route => route
                    //    .HttpFunction<SimpleHttpRouteCommand>())
                    .Storage("storageaccount", storage => storage
                        //.QueueFunction<StorageQueueCommand>("storagequeuecommandqueue")
                        //.BlobFunction<BlobCommand>("blobcommandcontainer/{name}") // TODO: We need to have the compiler insert parameters on the function for everything surrounded in {} - e.g. {name} needs a string parameter of name
                        .BlobFunction<StreamBlobCommand>("streamblobcommandcontainer/{name}")
                    )
                );
        }
    }
}
