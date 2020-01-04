using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Common.Services;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.TestInfrastructure
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
