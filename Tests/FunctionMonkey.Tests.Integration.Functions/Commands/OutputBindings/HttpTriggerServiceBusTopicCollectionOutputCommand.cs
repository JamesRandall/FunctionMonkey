using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class HttpTriggerServiceBusTopicCollectionOutputCommand : ICommand<IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>>
    {
        public Guid MarkerId { get; set; }
    }
}