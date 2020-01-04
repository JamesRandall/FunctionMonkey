using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
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