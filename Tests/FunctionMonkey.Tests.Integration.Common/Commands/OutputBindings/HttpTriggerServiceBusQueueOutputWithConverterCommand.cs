using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerServiceBusQueueOutputWithConverterCommand : IOptionalValueCommand, ICommand<ServiceBusQueuedMarkerIdCommand>
    {
        public Guid MarkerId { get; set; }
        
        public int? Value { get; set; }
    }
}