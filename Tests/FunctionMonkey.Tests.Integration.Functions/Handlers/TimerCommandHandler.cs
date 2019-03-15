using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class TimerCommandHandler : ICommandHandler<TimerCommand>
    {
        public Task ExecuteAsync(TimerCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
