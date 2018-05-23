using System;

namespace FunctionMonkey.Abstractions.Contexts
{
    public class StorageQueueContext
    {
        public DateTimeOffset ExpirationTime { get; set; }
        public DateTimeOffset InsertionTime { get; set; }
        public DateTimeOffset NextVisibleTime { get; set; }
        public string QueueTrigger { get; set; }
        public string Id { get; set; }
        public string PopReceipt { get; set; }
        public int DequeueCount { get; set; }
    }
}
