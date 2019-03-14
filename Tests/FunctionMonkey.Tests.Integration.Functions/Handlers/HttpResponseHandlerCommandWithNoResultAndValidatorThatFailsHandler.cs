using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Exceptions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpResponseHandlerCommandWithNoResultAndValidatorThatFailsHandler : ICommandHandler<HttpResponseHandlerCommandWithNoResultAndValidatorThatFails>
    {
        public Task ExecuteAsync(HttpResponseHandlerCommandWithNoResultAndValidatorThatFails command)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
