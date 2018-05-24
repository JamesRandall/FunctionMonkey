using System;
using System.Collections.Generic;
using System.Text;
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
                        .QueueFunction<StorageQueueCommand>("storagequeuecommandqueue")
                        //.BlobFunction<BlobCommand>("blobcommandcontainer")
                        //.BlobFunction<StreamBlobCommand>("streamblobcommandcontainer")
                    )
                );
        }
    }
}
