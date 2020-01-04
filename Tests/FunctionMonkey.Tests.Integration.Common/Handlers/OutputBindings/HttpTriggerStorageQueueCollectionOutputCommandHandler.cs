using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    internal class HttpTriggerStorageQueueCollectionOutputCommandHandler : ICommandHandler<HttpTriggerStorageQueueCollectionOutputCommand, IReadOnlyCollection<StorageQueuedMarkerIdCommand>>
    {
        public Task<IReadOnlyCollection<StorageQueuedMarkerIdCommand>> ExecuteAsync(HttpTriggerStorageQueueCollectionOutputCommand command, IReadOnlyCollection<StorageQueuedMarkerIdCommand> previousResult)
        {
            return StorageQueuedMarkerIdCommand.SuccessCollection(command.MarkerId);
        }
    }
}
