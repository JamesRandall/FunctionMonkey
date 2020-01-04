using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;
using FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    internal class HttpTriggerCosmosOutputCommandHandler : ICommandHandler<HttpTriggerCosmosOutputCommand, CosmosResponse>
    {
        public Task<CosmosResponse> ExecuteAsync(HttpTriggerCosmosOutputCommand command, CosmosResponse previousResult)
        {
            return Task.FromResult(new CosmosResponse()
            {
                MarkerId = command.MarkerId,
                Id = command.MarkerId.ToString()
            });
        }
    }
}
