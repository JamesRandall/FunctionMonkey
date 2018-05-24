using System;
using System.Collections.Generic;
using System.Threading;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Contexts;

namespace FunctionMonkey.Infrastructure
{
    class ContextManager : IContextSetter, IContextProvider
    {
        private static readonly AsyncLocal<ServiceBusContext> ServiceBusContextLocal = new AsyncLocal<ServiceBusContext>();

        private static readonly AsyncLocal<StorageQueueContext> StorageQueueContextLocal = new AsyncLocal<StorageQueueContext>();

        private static readonly AsyncLocal<BlobContext> BlobContextLocal = new AsyncLocal<BlobContext>();

        private static readonly AsyncLocal<EventHubContext> EventHubContextLocal = new AsyncLocal<EventHubContext>();

        void IContextSetter.SetServiceBusContext(int deliveryCount, DateTime enqueuedTimeUtc, string messageId)
        {
            ServiceBusContextLocal.Value = new ServiceBusContext
            {
                DeliveryCount = deliveryCount,
                EnqueuedTimeUTc = enqueuedTimeUtc,
                MessageId = messageId
            };
        }

        public void SetStorageQueueContext(DateTimeOffset expirationTime, DateTimeOffset insertionTime, DateTimeOffset nextVisibleTime,
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

        public void SetBlobContext(string blobTrigger, Uri uri, IDictionary<string, string> metadata)
        {
            BlobContextLocal.Value = new BlobContext
            {
                BlobTrigger = blobTrigger,
                Uri = uri,
                Metadata = metadata
            };
        }

        public void SetEventHubContext(DateTime enqueuedTimeUtc, long sequenceNumber, string offset)
        {
            EventHubContextLocal.Value = new EventHubContext
            {
                EnqueuedTimeUtc = enqueuedTimeUtc,
                Offset = offset,
                SequenceNumber = sequenceNumber
            };
        }

        public ServiceBusContext ServiceBusContext => ServiceBusContextLocal.Value;
        public StorageQueueContext StorageQueueContext => StorageQueueContextLocal.Value;
        public BlobContext BlobContext => BlobContextLocal.Value;
        public EventHubContext EventHubContext => EventHubContextLocal.Value;
    }
}
