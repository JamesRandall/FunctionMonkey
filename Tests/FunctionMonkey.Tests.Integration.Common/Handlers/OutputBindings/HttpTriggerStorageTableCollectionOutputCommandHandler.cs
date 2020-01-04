using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Common.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    class HttpTriggerStorageTableCollectionOutputCommandHandler : ICommandHandler<HttpTriggerStorageTableCollectionOutputCommand, IEnumerable<MarkerTableEntity>>
    {
        public Task<IEnumerable<MarkerTableEntity>> ExecuteAsync(HttpTriggerStorageTableCollectionOutputCommand command, IEnumerable<MarkerTableEntity> previousResult)
        {
            return Task.FromResult((IEnumerable<MarkerTableEntity>) new[]
            {
                new MarkerTableEntity {PartitionKey = command.MarkerId.ToString(), RowKey = string.Empty}
            });
        }
    }
}
