using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    public class HttpTriggerServiceBusTopicOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusTopicOutputCommand, ServiceBusQueuedMarkerIdCommand>
    {
        public Task<ServiceBusQueuedMarkerIdCommand> ExecuteAsync(HttpTriggerServiceBusTopicOutputCommand command, ServiceBusQueuedMarkerIdCommand previousResult)
        {
            return ServiceBusQueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}