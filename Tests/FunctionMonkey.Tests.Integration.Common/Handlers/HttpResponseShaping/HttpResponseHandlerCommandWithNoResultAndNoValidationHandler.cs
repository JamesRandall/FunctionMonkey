using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithNoResultAndNoValidationHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndNoValidation>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndNoValidation command)
        {
            return Task.CompletedTask;
        }
    }
}
