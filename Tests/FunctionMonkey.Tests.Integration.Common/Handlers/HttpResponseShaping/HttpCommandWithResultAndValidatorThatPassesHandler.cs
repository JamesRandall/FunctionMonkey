using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpCommandWithResultAndValidatorThatPassesHandler : ICommandHandler<HttpCommandWithResultAndValidatorThatPasses, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpCommandWithResultAndValidatorThatPasses command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}
