using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using MultiAssemblySample.Commands;

namespace MultiAssemblySample.Application.Handlers
{
    class SimpleCommandHandler : ICommandHandler<SimpleCommand>
    {
        public Task ExecuteAsync(SimpleCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
