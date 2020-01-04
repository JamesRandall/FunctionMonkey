using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    internal class HttpCommandWithNoResultAndNoValidationHandler : ICommandHandler<HttpCommandWithNoResultAndNoValidation>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndNoValidation command)
        {
            return Task.CompletedTask;
        }
    }
}
