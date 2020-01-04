using System.Linq;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class HttpListQueryStringParamCommandHandler : ICommandHandler<HttpListQueryStringParamCommand, int>
    {
        public Task<int> ExecuteAsync(HttpListQueryStringParamCommand command, int previousResult)
        {
            int[] values = command.Value.Select(int.Parse).ToArray();
            return Task.FromResult(values.Sum());
        }
    }
}