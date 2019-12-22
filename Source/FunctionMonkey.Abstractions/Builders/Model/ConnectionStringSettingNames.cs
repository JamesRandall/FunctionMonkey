namespace FunctionMonkey.Abstractions.Builders.Model
{
    public class ConnectionStringSettingNames
    {
        public string ServiceBus { get; set; } = "serviceBusConnectionString";

        public string Storage { get; set; } = "storageConnectionString";

        public string CosmosDb { get; set; } = "cosmosConnectionString";

        public string SignalR { get; set; } = "signalRConnectionString";

        public string EventHub { get; set; } = "eventHubConnectionString";
    }
}
