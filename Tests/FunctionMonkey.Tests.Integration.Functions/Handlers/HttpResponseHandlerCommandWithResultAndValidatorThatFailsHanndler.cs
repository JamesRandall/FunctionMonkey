using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Exceptions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpResponseHandlerCommandWithResultAndValidatorThatFailsHanndler : ICommandHandler<HttpResponseHandlerCommandWithResultAndValidatorThatFails, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpResponseHandlerCommandWithResultAndValidatorThatFails command, SimpleResponse previousResult)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
