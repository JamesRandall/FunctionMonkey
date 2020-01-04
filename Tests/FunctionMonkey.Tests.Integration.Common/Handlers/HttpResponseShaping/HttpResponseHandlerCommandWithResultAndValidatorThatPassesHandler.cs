using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithResultAndValidatorThatPassesHandler : ICommandHandler<HttpResponseHandlerCommandWithResultAndValidatorThatPasses, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpResponseHandlerCommandWithResultAndValidatorThatPasses command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}
