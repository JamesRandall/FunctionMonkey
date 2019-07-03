using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class HttpIEnumerableQueryParamCommandHandler : ICommandHandler<HttpIEnumerableQueryParamCommand, int>
    {
        public Task<int> ExecuteAsync(HttpIEnumerableQueryParamCommand command, int previousResult)
        {
            return Task.FromResult(command.Value.Sum());
        }
    }
}