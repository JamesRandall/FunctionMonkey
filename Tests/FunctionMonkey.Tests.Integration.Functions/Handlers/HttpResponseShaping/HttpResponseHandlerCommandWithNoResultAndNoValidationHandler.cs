using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithNoResultAndNoValidationHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndNoValidation>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndNoValidation command)
        {
            return Task.CompletedTask;
        }
    }
}
