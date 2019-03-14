using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpCommandWithNoResultAndValidatorThatPassesHandler : ICommandHandler<HttpCommandWithNoResultAndValidatorThatPasses>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndValidatorThatPasses command)
        {
            return Task.CompletedTask;
        }
    }
}
