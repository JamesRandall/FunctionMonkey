using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;

namespace SwaggerBuildOut.Handlers
{
    class CosmosDocumentBatchCommandHandler : ICommandHandler<CosmosDocumentBatchCommand>
    {
        public Task ExecuteAsync(CosmosDocumentBatchCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
