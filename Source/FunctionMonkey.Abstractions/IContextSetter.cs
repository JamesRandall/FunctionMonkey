using System;

namespace FunctionMonkey.Abstractions
{
    public interface IContextSetter
    {
        void SetServiceBusContext(int deliveryCount, DateTime enqueuedTimeUtc, string messageId);
    }
}
