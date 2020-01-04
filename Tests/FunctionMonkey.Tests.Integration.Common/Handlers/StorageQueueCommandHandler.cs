using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Extensions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class StorageQueueCommandHandler : ICommandHandler<StorageQueueCommand>
    {
        public async Task ExecuteAsync(StorageQueueCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
