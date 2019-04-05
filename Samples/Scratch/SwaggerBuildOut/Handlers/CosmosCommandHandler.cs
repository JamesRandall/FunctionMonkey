using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using SwaggerBuildOut.Commands;

namespace SwaggerBuildOut.Handlers
{
    class CosmosCommandHandler : ICommandHandler<CosmosCommand>
    {
        public Task ExecuteAsync(CosmosCommand command)
        {
            throw new Exception("eeek");
        }
    }
}
