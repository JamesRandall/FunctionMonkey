using System;
using System.Threading;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Contexts;

namespace FunctionMonkey.Infrastructure
{
    class ContextManager : IContextSetter, IContextProvider
    {
        private static readonly AsyncLocal<ServiceBusContext> ServiceBusContextLocal = new AsyncLocal<ServiceBusContext>();

        void IContextSetter.SetServiceBusContext(int deliveryCount, DateTime enqueuedTimeUtc, string messageId)
        {
            ServiceBusContextLocal.Value = new ServiceBusContext
            {
                DeliveryCount = deliveryCount,
                EnqueuedTimeUTc = enqueuedTimeUtc,
                MessageId = messageId
            };
        }

        public ServiceBusContext ServiceBusContext => ServiceBusContextLocal.Value;
    }
}
