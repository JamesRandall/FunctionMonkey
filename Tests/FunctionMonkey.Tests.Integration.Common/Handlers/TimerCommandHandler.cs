using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class TimerCommandHandler : ICommandHandler<TimerCommand>
    {
        public Task ExecuteAsync(TimerCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
