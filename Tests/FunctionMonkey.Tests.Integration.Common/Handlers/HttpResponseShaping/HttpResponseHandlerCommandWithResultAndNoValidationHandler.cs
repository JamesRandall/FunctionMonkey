using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithResultAndNoValidationHandler : ICommandHandler<HttpResponseHandlerCommandWithResultAndNoValidation, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpResponseHandlerCommandWithResultAndNoValidation command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}
