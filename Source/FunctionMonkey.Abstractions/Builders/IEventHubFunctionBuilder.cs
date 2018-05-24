using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IEventHubFunctionBuilder
    {
        IServiceBusFunctionBuilder EventHubFunction<TCommand>(string eventHubName) where TCommand : ICommand;
    }
}
