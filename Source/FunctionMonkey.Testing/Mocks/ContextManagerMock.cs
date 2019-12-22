using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Contexts;
using ExecutionContext = FunctionMonkey.Abstractions.Contexts.ExecutionContext;

namespace FunctionMonkey.Testing.Mocks
{
    /// <summary>
    /// This is essentially a clone of the ContextManager class from Function Monkey. The AsyncLocal is still needed to scope
    /// any set contexts when tests are running in parallel
    /// </summary>
    public class ContextManagerMock : IContextSetter, IContextProvider
    {
        private static readonly AsyncLocal<ServiceBusContext> ServiceBusContextLocal = new AsyncLocal<ServiceBusContext>();

        private static readonly AsyncLocal<StorageQueueContext> StorageQueueContextLocal = new AsyncLocal<StorageQueueContext>();

        private static readonly AsyncLocal<BlobContext> BlobContextLocal = new AsyncLocal<BlobContext>();

        private static readonly AsyncLocal<EventHubContext> EventHubContextLocal = new AsyncLocal<EventHubContext>();

        private static readonly AsyncLocal<ExecutionContext> ExecutionContextLocal = new AsyncLocal<ExecutionContext>();

        private static readonly AsyncLocal<HttpContext> HttpContextLocal = new AsyncLocal<HttpContext>();

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
                PartitionKey = partitionKey
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
