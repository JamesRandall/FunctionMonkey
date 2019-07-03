using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    public class HttpIReadOnlyCollectionQueryParamCommandHandler : ICommandHandler<HttpIReadOnlyCollectionQueryParamCommand, int>
    {
        public Task<int> ExecuteAsync(HttpIReadOnlyCollectionQueryParamCommand command, int previousResult)
        {
            return Task.FromResult(command.Value.Sum());
        }
    }
}