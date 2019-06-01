using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class ServiceBusSessionIdSubscriptionCommand : ICommand
    {
        public Guid SessionId { get; set; }

        public Guid MarkerId { get; set; }
    }
}
