using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerServiceBusQueueCollectionOutputCommand : ICommand<IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>>
    {
        public Guid MarkerId { get; set; }
    }
}