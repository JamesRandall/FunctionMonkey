using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.EventHubs
{
    public class EventHubCommand : ICommand
    {
        public Guid MarkerId { get; set; }
    }
}
