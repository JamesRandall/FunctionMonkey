using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;
using FunctionMonkey.Tests.Integration.Functions.Extensions;
using FunctionMonkey.Tests.Integration.Functions.Services;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.TestInfrastructure
{
    internal class EventHubQueuedMarkerIdCommandHandler : ICommandHandler<EventHubQueuedMarkerIdCommand>
    {
        private readonly IMarker _marker;

        public EventHubQueuedMarkerIdCommandHandler(IMarker marker)
        {
            _marker = marker;
        }
        
        public async Task ExecuteAsync(EventHubQueuedMarkerIdCommand command)
        {
            await _marker.RecordMarker(command.MarkerId);
        }
    }
}
