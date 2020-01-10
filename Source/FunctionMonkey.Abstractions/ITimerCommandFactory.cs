using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    public interface ITimerCommandFactory<out TCommand>
    {
        TCommand Create(string cronExpression);
    }
}
