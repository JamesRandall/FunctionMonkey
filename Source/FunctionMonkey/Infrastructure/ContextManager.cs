using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Contexts;
using ExecutionContext = FunctionMonkey.Abstractions.Contexts.ExecutionContext;

namespace FunctionMonkey.Infrastructure
{
    internal class ContextManager : IContextSetter, IContextProvider
    {
        // These are internal to allow the F# view of the world to gain access to them.
        // May refactor in the future though the F# package conceptually forms part of this
        // domain.
        internal static readonly AsyncLocal<ServiceBusContext> ServiceBusContextLocal = new AsyncLocal<ServiceBusContext>();

        internal static readonly AsyncLocal<StorageQueueContext> StorageQueueContextLocal = new AsyncLocal<StorageQueueContext>();

        internal static readonly AsyncLocal<BlobContext> BlobContextLocal = new AsyncLocal<BlobContext>();

        internal static readonly AsyncLocal<EventHubContext> EventHubContextLocal = new AsyncLocal<EventHubContext>();

        internal static readonly AsyncLocal<ExecutionContext> ExecutionContextLocal = new AsyncLocal<ExecutionContext>();

        internal static readonly AsyncLocal<HttpContext> HttpContextLocal = new AsyncLocal<HttpContext>();

        void IContextSetter.SetServiceBusContext(int deliveryCount, DateTime enqueuedTimeUtc, string messageId, string lockToken)
        {
            ServiceBusContextLocal.Value = new ServiceBusContext
            {
                DeliveryCount = deliveryCount,
                EnqueuedTimeUTc = enqueuedTimeUtc,
                MessageId = messageId,
                LockToken = lockToken
            };
        }

        void IContextSetter.SetStorageQueueContext(DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime,
            string queueTrigger, string id, string popReceipt, int dequeueCount)
        {
            StorageQueueContextLocal.Value = new StorageQueueContext
            {
                DequeueCount = dequeueCount,
                ExpirationTime = expirationTime,
                Id = id,
                InsertionTime = insertionTime,
                NextVisibleTime = nextVisibleTime,
                PopReceipt = popReceipt,
                QueueTrigger = queueTrigger
            };
        }

        void IContextSetter.SetBlobContext(string blobTrigger, Uri uri, IDictionary<string, string> metadata)
        {
            BlobContextLocal.Value = new BlobContext
            {
                BlobTrigger = blobTrigger,
                Uri = uri,
                Metadata = metadata
            };
        }

        void IContextSetter.SetEventHubContext(
            DateTime enqueuedTimeUtc,
            long sequenceNumber,
            string offset,
            string partitionKey)
        {
            EventHubContextLocal.Value = new EventHubContext
            {
                EnqueuedTimeUtc = enqueuedTimeUtc,
                Offset = offset,
                SequenceNumber = sequenceNumber,
                PartitionKey = partitionKey,
            };
        }

        void IContextSetter.SetExecutionContext(string functionAppDirectory, string functionDirectory, string functionName, Guid invocationId)
        {
            ExecutionContextLocal.Value = new ExecutionContext
            {
                FunctionDirectory = functionDirectory,
                FunctionAppDirectory = functionAppDirectory,
                FunctionName = functionName,
                InvocationId = invocationId
            };
        }

        public void SetHttpContext(ClaimsPrincipal claimsPrincipal, string requestUrl, Dictionary<string, IReadOnlyCollection<string>> headers)
        {
            HttpContextLocal.Value = new HttpContext
            {
                ClaimsPrincipal = claimsPrincipal,
                RequestUrl = requestUrl,
                Headers = headers
            };
        }

        public ServiceBusContext ServiceBusContext => ServiceBusContextLocal.Value;
        public StorageQueueContext StorageQueueContext => StorageQueueContextLocal.Value;
        public BlobContext BlobContext => BlobContextLocal.Value;
        public EventHubContext EventHubContext => EventHubContextLocal.Value;
        public ExecutionContext ExecutionContext => ExecutionContextLocal.Value;
        public HttpContext HttpContext => HttpContextLocal.Value;
    }
}
