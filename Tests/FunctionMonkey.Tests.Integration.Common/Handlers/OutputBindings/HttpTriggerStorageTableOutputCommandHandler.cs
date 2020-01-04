using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    internal class HttpTriggerStorageTableOutputCommandHandler : ICommandHandler<HttpTriggerStorageTableOutputCommand, MarkerTableEntity>
    {
        public Task<MarkerTableEntity> ExecuteAsync(HttpTriggerStorageTableOutputCommand command, MarkerTableEntity previousResult)
        {
            return MarkerTableEntity.Success(command.MarkerId);
        }
    }
}