using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class HttpTriggerStorageQueueCollectionOutputCommand : ICommand<IReadOnlyCollection<StorageQueuedMarkerIdCommand>>
    {
        public Guid MarkerId { get; set; }
    }
}
