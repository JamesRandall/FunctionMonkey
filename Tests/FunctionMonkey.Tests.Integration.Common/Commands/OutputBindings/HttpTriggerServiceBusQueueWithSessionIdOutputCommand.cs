using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerServiceBusQueueWithSessionIdOutputCommand : ICommand
    {
        public Guid MarkerId { get; set; }
        
        public Guid ASessionId { get; set; }
    }
}