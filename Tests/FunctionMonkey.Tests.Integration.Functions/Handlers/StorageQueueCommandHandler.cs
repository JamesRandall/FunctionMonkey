using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class StorageQueueCommandHandler : ICommandHandler<StorageQueueCommand>
    {
        public async Task ExecuteAsync(StorageQueueCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
