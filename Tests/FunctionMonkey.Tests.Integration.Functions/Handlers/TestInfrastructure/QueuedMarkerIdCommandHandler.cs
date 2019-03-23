using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.TestInfrastructure
{
    internal class QueuedMarkerIdCommandHandler : ICommandHandler<QueuedMarkerIdCommand>
    {
        public async Task ExecuteAsync(QueuedMarkerIdCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
