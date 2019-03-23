using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    class HttpTriggerServiceBusTopicCollectionOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusTopicCollectionOutputCommand, IReadOnlyCollection<QueuedMarkerIdCommand>>
    {
        public Task<IReadOnlyCollection<QueuedMarkerIdCommand>> ExecuteAsync(HttpTriggerServiceBusTopicCollectionOutputCommand command, IReadOnlyCollection<QueuedMarkerIdCommand> previousResult)
        {
            return QueuedMarkerIdCommand.SuccessCollection(command.MarkerId);
        }
    }
}
