using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    internal class HttpTriggerStorageQueueOutputCommandHandler : ICommandHandler<HttpTriggerStorageQueueOutputCommand, StorageQueuedMarkerIdCommand>
    {
        public Task<StorageQueuedMarkerIdCommand> ExecuteAsync(HttpTriggerStorageQueueOutputCommand command, StorageQueuedMarkerIdCommand previousResult)
        {
            return StorageQueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}