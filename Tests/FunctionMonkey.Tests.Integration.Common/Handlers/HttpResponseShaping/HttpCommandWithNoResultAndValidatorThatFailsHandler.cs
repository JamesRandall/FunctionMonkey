using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Exceptions;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    internal class HttpCommandWithNoResultAndValidatorThatFailsHandler : ICommandHandler<HttpCommandWithNoResultAndValidatorThatFails>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndValidatorThatFails command)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
