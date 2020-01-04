using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class ServiceBusSessionIdQueueCommandHandler : ICommandHandler<ServiceBusSessionIdQueueCommand>
    {
        public async Task ExecuteAsync(ServiceBusSessionIdQueueCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
