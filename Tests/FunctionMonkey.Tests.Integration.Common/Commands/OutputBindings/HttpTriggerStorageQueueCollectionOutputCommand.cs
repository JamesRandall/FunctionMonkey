using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerStorageQueueCollectionOutputCommand : ICommand<IReadOnlyCollection<StorageQueuedMarkerIdCommand>>
    {
        public Guid MarkerId { get; set; }
    }
}
