using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpResponseHandlerCommandWithNoResultAndNoValidationHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndNoValidation>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndNoValidation command)
        {
            return Task.CompletedTask;
        }
    }
}
