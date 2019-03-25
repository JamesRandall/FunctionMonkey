# Output Bindings

Function Monkey supports output bindings on all of its function types - a prerequisite for using them is that your command must return a result. For example:

    public class OutputDocument
    {
        public string Message { get; set; }

        public int Value { get; set; }
    }

    public class ExampleOutputCommand : ICommand<OutputDocument>
    {

    }

To use this command with a HTTP trigger and output the result to the Service Bus you would use the following configuration (note tthis example uses [default connection strings](/crosscutting/connectionStrings.md)):

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .HttpRoute("outputExample", route => route
                        .HttpFunction<ExampleOutputCommand>()
                        .OutputTo.ServiceBusQueue("myqueue")
                    )
                );
        }
    }

All the output bindings are available through the .OutputTo property on each Function and include:

* Service Bus Queue
* Service Bus Topic
* Storage Queue
* Storage Table
* Cosmos
* SignalR (both message and action)

An example of each in use is shown below:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .HttpRoute("outputExample", route => route
                        .HttpFunction<ExampleOutputCommand>()
                        .OutputTo.ServiceBusQueue("myqueue")
                    )

                    .ServiceBus(serviceBus => serviceBus
                        .QueueFunction<ServiceBusQueueTriggerTableOutputCommand>("myqueue")
                        .OutputTo.ServiceBusTopic("mytopic")

                        .SubscriptionFunction<ServiceBusSubscriptionMarkerIdCommand>("mytopic", "mysubscription")
                        .OutputTo.CosmosDb("mycollection", "mydatabase")

                        .QueueFunction<SendSignalRMessage>("signalRMessages")
                        .OutputTo.SignalRMessage("myHub")
                    )

                    .HttpRoute("addUserToGroup", route => route
                        .HttpFunction<AddUserToSignalRGroupCommand>(HttpMethod.Put)
                        .OutputTo.SignalRGroup("myHub")
                    )

                    .CosmosDb(cosmos => cosmos
                        .ChangeFeedFunction<CosmosTriggerTableOutputCommand>("mycollection", "mydatabase)
                        .OutputTo.StorageTable("mystoragetable)
                    )

                    .Storage(storage => storage
                        .BlobFunction<BlogTriggerTableOutputCommand>($"{Constants.Storage.Blob.BlobOutputCommandContainer}/{{name}}")
                        .OutputTo.StorageQueue("mystoragequeue")
                );
        }
    }

    Note that there are some specific command result types required for SignalR. More details can be found in its own section.