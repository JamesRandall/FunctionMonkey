using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    public class HttpTriggerEventHubOutputCommandHandler : ICommandHandler<HttpTriggerEventHubOutputCommand, EventHubQueuedMarkerIdCommand>
    {
        public Task<EventHubQueuedMarkerIdCommand> ExecuteAsync(HttpTriggerEventHubOutputCommand command, EventHubQueuedMarkerIdCommand previousResult)
        {
            return EventHubQueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}