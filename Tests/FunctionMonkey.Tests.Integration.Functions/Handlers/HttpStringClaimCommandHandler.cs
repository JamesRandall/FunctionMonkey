using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    public class HttpStringClaimCommandHandler : ICommandHandler<HttpStringClaimCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpStringClaimCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse()
            {
                Message = command.StringClaim
            });
        }
    }
}