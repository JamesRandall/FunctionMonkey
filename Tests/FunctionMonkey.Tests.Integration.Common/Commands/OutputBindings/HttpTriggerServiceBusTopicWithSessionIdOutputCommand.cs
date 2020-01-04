using System;
using FunctionMonkey.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerServiceBusTopicWithSessionIdOutputCommand : ICommandWithNoHandler
    {
        public Guid MarkerId { get; set; }
        
        public Guid ASessionId { get; set; }
    }
}