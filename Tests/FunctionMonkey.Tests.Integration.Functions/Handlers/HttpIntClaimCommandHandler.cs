using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    public class HttpIntClaimCommandHandler : ICommandHandler<HttpIntClaimCommand, SimpleResponse>
    {
        public Task<SimpleResponse> ExecuteAsync(HttpIntClaimCommand command, SimpleResponse previousResult)
        {
            return Task.FromResult(new SimpleResponse()
            {
                Value = command.MappedValue
            });
        }
    }
}