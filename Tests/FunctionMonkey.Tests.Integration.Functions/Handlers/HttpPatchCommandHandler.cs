using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpPatchCommandHandler : ICommandHandler<HttpPatchCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpPatchCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse
            {
                Message = command.Message,
                Value = command.Value
            });
        }
    }
}
