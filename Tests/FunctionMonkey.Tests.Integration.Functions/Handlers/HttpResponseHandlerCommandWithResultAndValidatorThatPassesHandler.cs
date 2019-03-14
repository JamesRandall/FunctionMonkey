using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpResponseHandlerCommandWithResultAndValidatorThatPassesHandler : ICommandHandler<HttpResponseHandlerCommandWithResultAndValidatorThatPasses, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpResponseHandlerCommandWithResultAndValidatorThatPasses command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}
