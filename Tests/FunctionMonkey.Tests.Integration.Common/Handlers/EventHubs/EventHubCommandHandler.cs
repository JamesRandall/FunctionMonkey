using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.EventHubs;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.EventHubs
{
    internal class EventHubCommandHandler : ICommandHandler<EventHubCommand>
    {
        public async Task ExecuteAsync(EventHubCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}