using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class HttpDefaultHeaderCommandHandler : ICommandHandler<HttpDefaultHeaderCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpDefaultHeaderCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse
            {
                Message = command.DefaultHeaderStringValue,
                Value = command.DefaultHeaderIntValue
            });
        }
    }
}