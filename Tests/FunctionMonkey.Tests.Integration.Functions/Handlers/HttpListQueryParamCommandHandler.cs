using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class HttpListQueryParamCommandHandler : ICommandHandler<HttpListQueryParamCommand, int>
    {
        public Task<int> ExecuteAsync(HttpListQueryParamCommand command, int previousResult)
        {
            return Task.FromResult(command.Value.Sum());
        }
    }
}