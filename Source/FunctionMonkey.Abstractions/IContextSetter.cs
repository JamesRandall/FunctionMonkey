using System;
using System.Collections.Generic;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Used by the compiled functions to set the context information available through IContextProvider
    /// </summary>
    public interface IContextSetter
    {
        /// <summary>
        /// Sets the service bus context
        /// </summary>
        void SetServiceBusContext(int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId);

        /// <summary>
        /// Sets the storage queue context
        /// </summary>
        void SetStorageQueueContext(DateTimeOffset expirationTime,
            DateTimeOffset insertionTime,
            DateTimeOffset nextVisibleTime,
            string queueTrigger,
            string id,
            string popReceipt,
            int dequeueCount);

        /// <summary>
        /// Sets the blob trigger context
        /// </summary>
        void SetBlobContext(string blobTrigger,
            Uri uri,
            IDictionary<string, string> metadata);

        /// <summary>
        /// Sets the event hub context
        /// </summary>
        void SetEventHubContext(DateTime enqueuedTimeUtc,
            Int64 sequenceNumber,
            string offset);

        /// <summary>
        /// Sets the execution 
        /// </summary>
        /// <param name="functionAppDirectory"></param>
        /// <param name="functionDirectory"></param>
        /// <param name="functionName"></param>
        /// <param name="invocationId"></param>
        void SetExecutionContext(string functionAppDirectory,
            string functionDirectory,
            string functionName,
            Guid invocationId);
    }
}
