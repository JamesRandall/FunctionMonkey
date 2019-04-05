using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    class HttpTriggerServiceBusTopicCollectionOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusTopicCollectionOutputCommand, IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>>
    {
        public Task<IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>> ExecuteAsync(HttpTriggerServiceBusTopicCollectionOutputCommand command, IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand> previousResult)
        {
            return ServiceBusQueuedMarkerIdCommand.SuccessCollection(command.MarkerId);
        }
    }
}
