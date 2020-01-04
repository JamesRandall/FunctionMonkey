using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpCommandWithNoResultAndValidatorThatPassesHandler : ICommandHandler<HttpCommandWithNoResultAndValidatorThatPasses>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndValidatorThatPasses command)
        {
            return Task.CompletedTask;
        }
    }
}
