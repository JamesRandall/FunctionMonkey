using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    public class HttpIReadOnlyCollectionQueryParamCommandHandler : ICommandHandler<HttpIReadOnlyCollectionQueryParamCommand, int>
    {
        public Task<int> ExecuteAsync(HttpIReadOnlyCollectionQueryParamCommand command, int previousResult)
        {
            return Task.FromResult(command.Value.Sum());
        }
    }
}