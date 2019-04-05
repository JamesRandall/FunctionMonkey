using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    public class HttpTriggerServiceBusQueueOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusQueueOutputCommand, ServiceBusQueuedMarkerIdCommand>
    {
        public Task<ServiceBusQueuedMarkerIdCommand> ExecuteAsync(HttpTriggerServiceBusQueueOutputCommand command, ServiceBusQueuedMarkerIdCommand previousResult)
        {
            return ServiceBusQueuedMarkerIdCommand.Success(command.MarkerId);
        }
    }
}