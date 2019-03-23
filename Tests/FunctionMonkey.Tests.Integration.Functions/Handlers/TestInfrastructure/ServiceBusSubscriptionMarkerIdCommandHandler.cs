using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.TestInfrastructure
{
    internal class ServiceBusSubscriptionMarkerIdCommandHandler : ICommandHandler<ServiceBusSubscriptionMarkerIdCommand>
    {
        public async Task ExecuteAsync(ServiceBusSubscriptionMarkerIdCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
