using System;

namespace FunctionMonkey.Abstractions.Contexts
{
    public class EventHubContext
    {
        public DateTime EnqueuedTimeUtc { get; set; }
        public Int64 SequenceNumber { get; set; }
        public string Offset { get; set; }
    }
}
