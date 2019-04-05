using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;
using FunctionMonkey.Tests.Integration.Functions.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
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
