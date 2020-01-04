using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;
using FunctionMonkey.Tests.Integration.Common.Exceptions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpCommandWithResultAndValidatorThatFailsHandler : ICommandHandler<HttpCommandWithResultAndValidatorThatFails, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpCommandWithResultAndValidatorThatFails command, SimpleResponse previousResult)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
