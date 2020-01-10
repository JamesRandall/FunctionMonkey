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
        /// <param name="isSessionEnabled">Should session IDs be used when processing the queue, requires a session ID enabled queue. Defaults to false.</param>
        /// <returns>A service bus function builder that allows more functions to be created</returns>
        IServiceBusFunctionOptionBuilder<TCommand> QueueFunction<TCommand>(string queueName, bool isSessionEnabled=false);

        /// <summary>
        /// Associate a function with the named topic and subscription and command type
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="topicName">The name of the topic</param>
        /// <param name="subscriptionName">The name of the subscription</param>
        /// /// <param name="isSessionEnabled">Should session IDs be used when processing the queue, requires a session ID enabled subscription. Defaults to false.</param>
        /// <returns>A service bus function builder that allows more functions to be created</returns>
        IServiceBusFunctionOptionBuilder<TCommand> SubscriptionFunction<TCommand>(string topicName, string subscriptionName, bool isSessionEnabled=false);
    }
}
