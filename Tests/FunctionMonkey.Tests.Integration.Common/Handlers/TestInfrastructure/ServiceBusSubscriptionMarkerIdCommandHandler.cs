using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.TestInfrastructure
{
    internal class ServiceBusSubscriptionMarkerIdCommandHandler : ICommandHandler<ServiceBusSubscriptionMarkerIdCommand>
    {
        public async Task ExecuteAsync(ServiceBusSubscriptionMarkerIdCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
