using AzureFromTheTrenches.Commanding.Abstractions;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Abstractions.Builders
{
    public interface IServiceBusFunctionBuilder
    {
        IServiceBusFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand;

        IServiceBusFunctionBuilder SubscriptionFunction<TCommand>(string topicName, string subscriptionName) where TCommand : ICommand;
    }
}
