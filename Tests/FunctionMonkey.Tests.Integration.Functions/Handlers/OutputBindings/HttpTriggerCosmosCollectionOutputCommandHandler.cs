using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    internal class HttpTriggerCosmosCollectionOutputCommandHandler : ICommandHandler<HttpTriggerCosmosCollectionOutputCommand, List<CosmosResponse>>
    {
        public Task<List<CosmosResponse>> ExecuteAsync(HttpTriggerCosmosCollectionOutputCommand command, List<CosmosResponse> previousResult)
        {
            return Task.FromResult(new List<CosmosResponse>()
            {
                new CosmosResponse()
                {
                    MarkerId = command.MarkerId,
                    Id = command.MarkerId.ToString()
                }
            });
        }
    }
}
