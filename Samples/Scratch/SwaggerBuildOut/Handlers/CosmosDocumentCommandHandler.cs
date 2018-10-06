using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;

namespace SwaggerBuildOut.Handlers
{
    internal class CosmosDocumentCommandHandler : ICommandHandler<CosmosDocumentCommand>
    {
        public Task ExecuteAsync(CosmosDocumentCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
