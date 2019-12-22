using System;
using System.Collections.Generic;
using System.Security.Claims;

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
            string messageId,
            string lockToken);

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
        void SetEventHubContext(
            DateTime enqueuedTimeUtc,
            long sequenceNumber,
            string offset,
            string partitionKey
            );

        /// <summary>
        /// Sets the execution context
        /// </summary>
        /// <param name="functionAppDirectory"></param>
        /// <param name="functionDirectory"></param>
        /// <param name="functionName"></param>
        /// <param name="invocationId"></param>
        void SetExecutionContext(string functionAppDirectory,
            string functionDirectory,
            string functionName,
            Guid invocationId);

        /// <summary>
        /// Sets the HTTP context
        /// </summary>
        /// <param name="requestUrl">The request URL</param>
        /// <param name="headers">The headers associated with the context</param>
        void SetHttpContext(ClaimsPrincipal claimsPrincipal, string requestUrl, Dictionary<string, IReadOnlyCollection<string>> headers);
    }
}
