using System;

namespace FunctionMonkey.Abstractions.Contexts
{
    public class ServiceBusContext
    {
        public int DeliveryCount { get; set; }

        public DateTime EnqueuedTimeUTc { get; set; }

        public string MessageId { get; set; }
    }
}
