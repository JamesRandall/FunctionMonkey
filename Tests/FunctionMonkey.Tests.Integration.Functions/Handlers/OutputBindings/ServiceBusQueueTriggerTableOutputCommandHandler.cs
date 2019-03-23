using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    class ServiceBusQueueTriggerTableOutputCommandHandler : ICommandHandler<ServiceBusQueueTriggerTableOutputCommand, MarkerTableEntity>
    {
        public Task<MarkerTableEntity> ExecuteAsync(ServiceBusQueueTriggerTableOutputCommand command, MarkerTableEntity previousResult)
        {
            return MarkerTableEntity.Success(command.MarkerId);
        }
    }
}
