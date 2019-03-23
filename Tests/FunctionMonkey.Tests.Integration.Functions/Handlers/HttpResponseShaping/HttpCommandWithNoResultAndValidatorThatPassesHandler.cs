using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.HttpResponseShaping
{
    class HttpCommandWithNoResultAndValidatorThatPassesHandler : ICommandHandler<HttpCommandWithNoResultAndValidatorThatPasses>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndValidatorThatPasses command)
        {
            return Task.CompletedTask;
        }
    }
}
