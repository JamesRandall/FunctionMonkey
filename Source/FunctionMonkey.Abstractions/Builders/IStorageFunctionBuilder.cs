using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IStorageFunctionBuilder
    {
        IStorageFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand;
    }
}
