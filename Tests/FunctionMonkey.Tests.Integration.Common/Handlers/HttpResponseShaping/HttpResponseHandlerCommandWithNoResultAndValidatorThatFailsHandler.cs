using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Exceptions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithNoResultAndValidatorThatFailsHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndValidatorThatFails>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndValidatorThatFails command)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
