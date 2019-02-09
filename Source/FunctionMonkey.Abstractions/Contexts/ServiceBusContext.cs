using System;

namespace FunctionMonkey.Abstractions.Contexts
{
    /// <summary>
    /// Context information for the service bus trigger
    /// </summary>
    public class ServiceBusContext
    {
        /// <summary>
        /// The number of times the message has been delivered
        /// </summary>
        public int DeliveryCount { get; set; }

        /// <summary>
        /// The date and time the message was enqueued
        /// </summary>
        public DateTime EnqueuedTimeUTc { get; set; }

        /// <summary>
        /// The ID of the message
        /// </summary>
        public string MessageId { get; set; }

        /// <summary>
        /// The lock token of the message
        /// </summary>
        public string LockToken { get; set; }
    }
}
