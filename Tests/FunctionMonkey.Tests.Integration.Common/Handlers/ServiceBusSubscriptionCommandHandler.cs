using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class ServiceBusSubscriptionCommandHandler : ICommandHandler<ServiceBusSubscriptionCommand>
    {
        public async Task ExecuteAsync(ServiceBusSubscriptionCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
