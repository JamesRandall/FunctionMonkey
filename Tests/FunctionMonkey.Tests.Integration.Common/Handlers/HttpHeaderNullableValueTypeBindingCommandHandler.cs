using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class HttpHeaderNullableValueTypeBindingCommandHandler : ICommandHandler<HttpHeaderNullableValueTypeBindingCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpHeaderNullableValueTypeBindingCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse
            {
                Value = command.Value.HasValue ? command.Value.Value : int.MinValue
            });
        }
    }
}
