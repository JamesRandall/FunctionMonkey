using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class HttpGetQueryParamCommandHandler : ICommandHandler<HttpGetQueryParamCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpGetQueryParamCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse
            {
                Message = command.NullableGuid?.ToString(),
                Value = command.Value
            });
        }
    }
}
