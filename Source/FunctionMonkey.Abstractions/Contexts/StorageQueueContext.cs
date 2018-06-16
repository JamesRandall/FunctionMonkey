using System;

namespace FunctionMonkey.Abstractions.Contexts
{
    /// <summary>
    /// Context information for Azure Storage queue triggered functions
    /// </summary>
    public class StorageQueueContext
    {
        /// <summary>
        /// The expiration time of the event 
        /// </summary>
        public DateTimeOffset ExpirationTime { get; set; }
        /// <summary>
        /// The insertion time of the message
        /// </summary>
        public DateTimeOffset InsertionTime { get; set; }
        /// <summary>
        /// The next time the message will be visible
        /// </summary>
        public DateTimeOffset NextVisibleTime { get; set; }
        /// <summary>
        /// The trigger
        /// </summary>
        public string QueueTrigger { get; set; }
        /// <summary>
        /// The message ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The pop recepit
        /// </summary>
        public string PopReceipt { get; set; }
        /// <summary>
        /// The dequeue count
        /// </summary>
        public int DequeueCount { get; set; }
    }
}
