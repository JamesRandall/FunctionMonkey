using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Handlers
{
    class SimpleHttpRouteCommandHandler : ICommandHandler<SimpleHttpRouteCommand>
    {
        public Task ExecuteAsync(SimpleHttpRouteCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
