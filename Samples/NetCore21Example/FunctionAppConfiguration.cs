using System;
using System.Collections.Generic;
using System.Text;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;

namespace NetCore21Example
{
    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((sc, r) => { r.Register<SimpleCommandHandler>(); })
                .OpenApiEndpoint(openApi => openApi.Title("My API").UserInterface().Version("1.0.0"))
                .Functions(functions => functions
                    .HttpRoute("api", route => route
                        .HttpFunction<SimpleCommand>()
                    )
                    .ServiceBus("myconnection", sb => sb.QueueFunction<SimpleCommand>("myqueue"))
                    .Storage("storageconnection", s => s.QueueFunction<SimpleCommand>("myqueue"))
                    .CosmosDb("mycosmos", c => c.ChangeFeedFunction<SimpleCommand>("collection", "database"))
                    .Timer<SimpleCommand>("1 * * * * *")
                );
        }
    }
}
