using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    class HttpTriggerServiceBusQueueCollectionOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusQueueCollectionOutputCommand, IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>>
    {
        public Task<IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>> ExecuteAsync(HttpTriggerServiceBusQueueCollectionOutputCommand command, IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand> previousResult)
        {
            return ServiceBusQueuedMarkerIdCommand.SuccessCollection(command.MarkerId);
        }
    }
}
