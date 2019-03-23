using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    public class HttpTriggerStorageQueueOutputCommandHandler : ICommandHandler<HttpTriggerStorageQueueOutputCommand, QueuedMarkerIdCommand>
    {
        public Task<QueuedMarkerIdCommand> ExecuteAsync(HttpTriggerStorageQueueOutputCommand command, QueuedMarkerIdCommand previousResult)
        {
            return QueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}