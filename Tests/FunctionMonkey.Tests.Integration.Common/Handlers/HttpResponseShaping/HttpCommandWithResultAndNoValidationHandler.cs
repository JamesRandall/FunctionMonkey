using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.HttpResponseShaping
{
    class HttpCommandWithResultAndNoValidationHandler : ICommandHandler<HttpCommandWithResultAndNoValidation, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpCommandWithResultAndNoValidation command, SimpleResponse previousResult)
        {
            return SimpleResponse.Success();
        }
    }
}
