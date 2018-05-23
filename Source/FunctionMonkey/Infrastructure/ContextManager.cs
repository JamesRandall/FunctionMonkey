using System;
using System.Threading;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Contexts;

namespace FunctionMonkey.Infrastructure
{
    class ContextManager : IContextSetter, IContextProvider
    {
        private static readonly AsyncLocal<ServiceBusContext> ServiceBusContextLocal = new AsyncLocal<ServiceBusContext>();

        private static readonly AsyncLocal<StorageQueueContext> StorageQueueContextLocal = new AsyncLocal<StorageQueueContext>();

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

        public ServiceBusContext ServiceBusContext => ServiceBusContextLocal.Value;
        public StorageQueueContext StorageQueueContext => StorageQueueContextLocal.Value;
    }
}
