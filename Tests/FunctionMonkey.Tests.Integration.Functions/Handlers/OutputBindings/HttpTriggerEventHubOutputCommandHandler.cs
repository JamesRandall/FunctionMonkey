using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    public class HttpTriggerEventHubOutputCommandHandler : ICommandHandler<HttpTriggerEventHubOutputCommand, EventHubQueuedMarkerIdCommand>
    {
        public Task<EventHubQueuedMarkerIdCommand> ExecuteAsync(HttpTriggerEventHubOutputCommand command, EventHubQueuedMarkerIdCommand previousResult)
        {
            return EventHubQueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}