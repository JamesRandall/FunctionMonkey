using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Functions.Exceptions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.HttpResponseShaping
{
    internal class HttpCommandWithNoResultAndValidatorThatFailsHandler : ICommandHandler<HttpCommandWithNoResultAndValidatorThatFails>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndValidatorThatFails command)
        {
            throw new HandlerShouldNotBeExecutedException();
        }
    }
}
