using System;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Contexts
{
    public class ServiceBusContext
    {
        public int DeliveryCount { get; set; }

        public DateTime EnqueuedTimeUTc { get; set; }

        public string MessageId { get; set; }
    }
}
