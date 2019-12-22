using System;
using System.Collections.Generic;

namespace FunctionMonkey.Abstractions.Contexts
{
    /// <summary>
    /// Event hub trigger context information
    /// </summary>
    public class EventHubContext
    {
        /// <summary>
        /// The date time the event was enqueued
        /// </summary>
        public DateTime EnqueuedTimeUtc { get; set; }
        /// <summary>
        /// The sequence number of the event
        /// </summary>
        public long SequenceNumber { get; set; }
        /// <summary>
        /// The serialized size of the event
        /// </summary>
        public long SerializedSizeInBytes { get; set; }
        /// <summary>
        /// The offset of the event
        /// </summary>
        public string Offset { get; set; }
        /// <summary>
        /// The partition key of the event
        /// </summary>
        public string PartitionKey { get; set; }
        /// <summary>
        /// The properties of the event
        /// </summary>
        public IDictionary<string,object> Properties { get; set; }
        /// <summary>
        /// The system properties of the event
        /// </summary>
        public IDictionary<string,object> SystemProperties { get; set; }
    }
}
