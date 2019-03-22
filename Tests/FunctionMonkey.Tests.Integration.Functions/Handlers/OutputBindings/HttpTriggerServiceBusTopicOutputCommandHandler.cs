using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    public class HttpTriggerServiceBusTopicOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusTopicOutputCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpTriggerServiceBusTopicOutputCommand command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}