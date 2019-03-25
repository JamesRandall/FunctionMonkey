# Connection Strings

Many trigger types require a conncetion string setting names to be supplied. For example using the Service Bus you may declare a queue function like this:

    .Functions(functions => functions
        .ServiceBus("serviceBusConnectionString", serviceBus => serviceBus
            .QueueFunction<SendEmailCommand>(QueueName)
        )
    );

An alternative approach is to rely on default connection string setting names (again using the Service Bus as an example but this applies to all trigger types):

    .Functions(functions => functions
        .ServiceBus(serviceBus => serviceBus
            .QueueFunction<SendEmailCommand>(QueueName)
        )
    );

The default connection string setting names are as follows (showed in a sample .settings.json):

    {
        "Values": {
            "FUNCTIONS_WORKER_RUNTIME": "dotnet",
            "AzureWebJobsStorage": "",
            "AzureWebJobsDashboard": "",

            "storageConnectionString": "default azure storage connection string - queues, blob, table",
            "cosmosConnectionString": "default cosmos connection string",
            "serviceBusConnectionString": "default service bus connection string",
            "signalRConnectionString": "default signalR connection string"
        }
    }

You can change the default setting names as shown in the following example (you don't have to set them all, just the ones you need):

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .DefaultConnectionStringSettingNames(settingNames =>
                    {
                        settingNames.Storage = "newStorageConnectionSettingName";
                        settingNames.CosmosDb = "newCosmosConnectionSettingName";
                        settingNames.ServiceBus = "newServiceBusConnectionSettingName";
                        settingNames.SignalR = "newSignalRConnectionSetingName";
                    })
                .Functions(...);
        }
    }
