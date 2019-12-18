using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Functions.Extensions;
using FunctionMonkey.Tests.Integration.Functions.Services;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.TestInfrastructure
{
    internal class ServiceBusQueuedMarkerIdCommandHandler : ICommandHandler<ServiceBusQueuedMarkerIdCommand>
    {
        private readonly IMarker _marker;

        public ServiceBusQueuedMarkerIdCommandHandler(IMarker marker)
        {
            _marker = marker;
        }
        
        public async Task ExecuteAsync(ServiceBusQueuedMarkerIdCommand command)
        {
            await _marker.RecordMarker(command.MarkerId);
        }
    }
}
