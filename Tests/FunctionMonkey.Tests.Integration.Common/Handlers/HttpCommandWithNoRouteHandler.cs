using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class HttpCommandWithNoRouteHandler : ICommandHandler<HttpCommandWithNoRoute>
    {
        public Task ExecuteAsync(HttpCommandWithNoRoute command)
        {
            return Task.CompletedTask;
        }
    }
}
