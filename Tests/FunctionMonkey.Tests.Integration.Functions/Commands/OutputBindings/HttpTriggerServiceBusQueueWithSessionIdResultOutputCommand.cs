using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class ModelWithSessionId
    {
        public Guid SessionId { get; set; }
    }
    
    public class HttpTriggerServiceBusQueueWithSessionIdResultOutputCommand : ICommand<ModelWithSessionId>
    {
        public Guid MarkerId { get; set; }
        
        public Guid ASessionId { get; set; }
    }
}