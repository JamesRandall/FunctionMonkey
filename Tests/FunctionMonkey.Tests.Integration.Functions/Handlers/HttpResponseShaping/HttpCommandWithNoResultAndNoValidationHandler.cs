using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.HttpResponseShaping
{
    internal class HttpCommandWithNoResultAndNoValidationHandler : ICommandHandler<HttpCommandWithNoResultAndNoValidation>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndNoValidation command)
        {
            return Task.CompletedTask;
        }
    }
}
