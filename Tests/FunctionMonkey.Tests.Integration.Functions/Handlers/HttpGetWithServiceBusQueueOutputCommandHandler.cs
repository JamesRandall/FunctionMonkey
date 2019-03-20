using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    public class HttpGetWithServiceBusQueueOutputCommandHandler : ICommandHandler<HttpGetWithServiceBusQueueOutputCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpGetWithServiceBusQueueOutputCommand command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}