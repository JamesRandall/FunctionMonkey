using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    /*internal class HttpTriggerStorageBlobOutputCommandResultCommandHandler : ICommandHandler<HttpTriggerStorageBlobOutputCommandResultCommand, NamedBlobOutputResponse>
    {
        public Task<NamedBlobOutputResponse> ExecuteAsync(HttpTriggerStorageBlobOutputCommandResultCommand command, NamedBlobOutputResponse previousResult)
        {
            return Task.FromResult(new NamedBlobOutputResponse
            {
                Name = $"{command.MarkerId}.json",
                Value = new BlobOutputResponse
                {
                    MarkerId = command.MarkerId
                }
            });
        }
    }*/
}
