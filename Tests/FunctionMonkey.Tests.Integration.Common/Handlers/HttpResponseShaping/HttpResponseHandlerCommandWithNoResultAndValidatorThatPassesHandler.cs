using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses command)
        {
            return Task.CompletedTask;
        }
    }
}
