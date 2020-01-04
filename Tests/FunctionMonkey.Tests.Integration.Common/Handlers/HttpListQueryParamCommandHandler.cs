using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class HttpListQueryParamCommandHandler : ICommandHandler<HttpListQueryParamCommand, int>
    {
        public Task<int> ExecuteAsync(HttpListQueryParamCommand command, int previousResult)
        {
            return Task.FromResult(command.Value.Sum());
        }
    }
}