using System;

namespace FunctionMonkey.Abstractions
{
    public interface IContextSetter
    {
        void SetServiceBusContext(int deliveryCount,
            DateTime enqueuedTimeUtc,
            string messageId);

        void SetStorageQueueContext(DateTimeOffset expirationTime,
            DateTimeOffset insertionTime,
            DateTimeOffset nextVisibleTime,
            string queueTrigger,
            string id,
            string popReceipt,
            int dequeueCount);
    }
}
