using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.EventHubs
{
    public class EventHubCommand : ICommand
    {
        public Guid MarkerId { get; set; }
    }
}
