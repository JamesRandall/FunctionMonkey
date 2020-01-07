using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    public class HttpGetCommandWithSecurityPropertyHandler : ICommandHandler<HttpGetCommandWithSecurityProperty, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpGetCommandWithSecurityProperty command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse
            {
                Message = command.Message,
                Value = command.Value
            });
        }
    }
}