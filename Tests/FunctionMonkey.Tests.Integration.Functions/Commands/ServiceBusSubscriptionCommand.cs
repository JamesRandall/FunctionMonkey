using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class ServiceBusSubscriptionCommand : ICommand
    {
        public Guid MarkerId { get; set; }
    }
}
