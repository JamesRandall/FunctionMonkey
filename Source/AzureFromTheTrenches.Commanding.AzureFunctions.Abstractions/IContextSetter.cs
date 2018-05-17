using System;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions
{
    public interface IContextSetter
    {
        void SetServiceBusContext(int deliveryCount, DateTime enqueuedTimeUtc, string messageId);
    }
}
