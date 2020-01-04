using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class ServiceBusSessionIdSubscriptionCommandHandler : ICommandHandler<ServiceBusSessionIdSubscriptionCommand>
    {
        public async Task ExecuteAsync(ServiceBusSessionIdSubscriptionCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
