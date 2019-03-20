using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    public class HttpGetWithServiceBusQueueOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusQueueOutputCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpTriggerServiceBusQueueOutputCommand command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}