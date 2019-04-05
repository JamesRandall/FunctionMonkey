using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithNoResultAndValidatorThatPassesHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses command)
        {
            return Task.CompletedTask;
        }
    }
}
