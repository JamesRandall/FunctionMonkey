using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;
using FunctionMonkey.Tests.Integration.Common.Exceptions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithResultAndValidatorThatFailsHanndler : ICommandHandler<HttpResponseHandlerCommandWithResultAndValidatorThatFails, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpResponseHandlerCommandWithResultAndValidatorThatFails command, SimpleResponse previousResult)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
