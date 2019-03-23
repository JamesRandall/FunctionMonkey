using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.HttpResponseShaping
{
    class HttpResponseHandlerCommandWithResultAndNoValidationHandler : ICommandHandler<HttpResponseHandlerCommandWithResultAndNoValidation, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpResponseHandlerCommandWithResultAndNoValidation command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}
