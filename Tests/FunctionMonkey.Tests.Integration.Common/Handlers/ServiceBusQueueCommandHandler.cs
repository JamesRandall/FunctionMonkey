using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class ServiceBusQueueCommandHandler : ICommandHandler<ServiceBusQueueCommand>
    {
        public async Task ExecuteAsync(ServiceBusQueueCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
