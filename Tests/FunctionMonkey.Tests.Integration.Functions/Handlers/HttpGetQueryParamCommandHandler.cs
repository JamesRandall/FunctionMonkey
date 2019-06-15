using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
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
