using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    public class HttpTriggerServiceBusQueueOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusQueueOutputCommand, QueuedMarkerIdCommand>
    {
        public Task<QueuedMarkerIdCommand> ExecuteAsync(HttpTriggerServiceBusQueueOutputCommand command, QueuedMarkerIdCommand previousResult)
        {
            return QueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}