namespace FunctionMonkey.Tests.Integration.Common
{
    public class Settings
    {
        public string Host { get; set; }

        public string StorageConnectionString { get; set; }

        public string ServiceBusConnectionString { get; set; }

        public string CosmosConnectionString { get; set; }
        
        public string EventHubConnectionString { get; set; }
    }
}
