using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    internal class HttpTriggerStorageQueueCollectionOutputCommandHandler : ICommandHandler<HttpTriggerStorageQueueCollectionOutputCommand, IReadOnlyCollection<StorageQueuedMarkerIdCommand>>
    {
        public Task<IReadOnlyCollection<StorageQueuedMarkerIdCommand>> ExecuteAsync(HttpTriggerStorageQueueCollectionOutputCommand command, IReadOnlyCollection<StorageQueuedMarkerIdCommand> previousResult)
        {
            return StorageQueuedMarkerIdCommand.SuccessCollection(command.MarkerId);
        }
    }
}
