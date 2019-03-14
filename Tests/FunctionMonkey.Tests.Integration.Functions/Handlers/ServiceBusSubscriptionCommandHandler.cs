using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class ServiceBusSubscriptionCommandHandler : ICommandHandler<ServiceBusSubscriptionCommand>
    {
        public async Task ExecuteAsync(ServiceBusSubscriptionCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
