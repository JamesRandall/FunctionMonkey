using System;

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
        public Int64 SequenceNumber { get; set; }
        /// <summary>
        /// The offset of the event
        /// </summary>
        public string Offset { get; set; }
    }
}
