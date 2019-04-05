using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
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
