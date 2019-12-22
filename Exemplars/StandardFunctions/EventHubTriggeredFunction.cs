using System;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
//using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

namespace StandardFunctions
{
    public static class EventHubTriggeredFunction
    {
        [FunctionName("EventHubTriggeredFunction")]
        public static void Run(
            [EventHubTrigger("samples-workitems", Connection = "eventHubConnectionString")]string myEventHubMessage,
            EventData eventData,
            ILogger log)
        {
            //log.Info($"C# Event Hub trigger function processed a message: {myEventHubMessage}");
        }
    }
}
