using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses command)
        {
            return Task.CompletedTask;
        }
    }
}
