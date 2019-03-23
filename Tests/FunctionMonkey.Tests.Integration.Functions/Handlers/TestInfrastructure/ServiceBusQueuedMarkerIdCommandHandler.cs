using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.TestInfrastructure
{
    internal class ServiceBusQueuedMarkerIdCommandHandler : ICommandHandler<ServiceBusQueuedMarkerIdCommand>
    {
        public async Task ExecuteAsync(ServiceBusQueuedMarkerIdCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
