using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpHeaderEnumTypeBindingCommandHandler : ICommandHandler<HttpHeaderEnumTypeBindingCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpHeaderEnumTypeBindingCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse
            {
                Value = (int)command.Value
            });
        }
    }
}
