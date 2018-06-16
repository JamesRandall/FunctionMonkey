using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// An interface that allows for the configuration of service bus triggered functions
    /// </summary>
    public interface IServiceBusFunctionBuilder
    {
        /// <summary>
        /// Associate a function with the named service bus queue and command type
        /// </summary>
        /// <typeparam name="TCommand">The command type</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>A service bus function builder that allows more functions to be created</returns>
        IServiceBusFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand;

        /// <summary>
        /// Associate a function with the named topic and subscription and command type
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="topicName">The name of the topic</param>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// <returns>A service bus function builder that allows more functions to be created</returns>
        IServiceBusFunctionBuilder SubscriptionFunction<TCommand>(string topicName, string subscriptionName) where TCommand : ICommand;
    }
}
