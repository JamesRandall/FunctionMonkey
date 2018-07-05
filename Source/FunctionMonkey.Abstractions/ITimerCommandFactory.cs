using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    public interface ITimerCommandFactory<out TCommand> where TCommand : ICommand
    {
        TCommand Create(string cronExpression);
    }
}
