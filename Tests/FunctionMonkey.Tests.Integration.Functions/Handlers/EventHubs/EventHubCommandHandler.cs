using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.EventHubs;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.EventHubs
{
    internal class EventHubCommandHandler : ICommandHandler<EventHubCommand>
    {
        public async Task ExecuteAsync(EventHubCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}