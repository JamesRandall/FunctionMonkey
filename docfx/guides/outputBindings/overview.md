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
* Event Hubs

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

                    .HttpRoute("recordMetric", route => route
                        .HttpFunction<RecordMetricCommand>(HttpMethod.Put)
                        .OutputTo.EventHub("myEventHub")
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

## Output Bindings without Command Handlers

If you simply want to accept a payload from a trigger and hand it straight off to an output trigger then a command handler is somewhat superfluous - it won't take the payload and return the payload. This can be useful if, for example, you have a HTTP endpoint for which you want to enforce authorization and validation before placing the payload onto a Service Bus queue.

Function Monkey makes it possible to use a command without creating and registering a command handler in one of two ways.

Firstly you can mark your command with the _ICommandWithNoHandler_ interface rather than the _ICommand_ interface.

    public class SampleCommand : ICommandWithNoHandler
    {
        public string SomeValue { get; set; }
    }

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .HttpRoute("outputExample", route => route
                        .HttpFunction<SampleCommand>()
                        .OutputTo.ServiceBusQueue("myqueue")
                    )
                );
        }
    }

Alternatively you can use the NoCommandHandler option as shown below:

    public class SampleCommand : ICommand
    {
        public string SomeValue { get; set; }
    }

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Functions(functions => functions
                    .HttpRoute("outputExample", route => route
                        .HttpFunction<SampleCommand>()
                        .Options(options => options.NoCommandHandler())
                        .OutputTo.ServiceBusQueue("myqueue")
                    )
                );
        }
    }

