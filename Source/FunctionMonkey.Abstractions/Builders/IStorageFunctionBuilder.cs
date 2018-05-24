using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions.Builders
{
    public interface IStorageFunctionBuilder
    {
        /// <summary>
        /// Creates a function for a storage queue
        /// </summary>
        /// <typeparam name="TCommand">The type of command</typeparam>
        /// <param name="queueName">The name of the queue</param>
        /// <returns>Builder for use in a fluent API</returns>
        IStorageFunctionBuilder QueueFunction<TCommand>(string queueName) where TCommand : ICommand;

        /// <summary>
        /// Creates a function for a blob trigger
        /// </summary>
        /// <typeparam name="TCommand">The type of the command. If the command implements IStreamCommand then
        /// it will be passed the blob as an open stream - otherwise the blob will be deserialized into
        /// the command</typeparam>
        /// <param name="blobPath">Container and optional pattern for the blob</param>
        /// <returns>Builder for use in a fluent API</returns>
        IStorageFunctionBuilder BlobFunction<TCommand>(string blobPath) where TCommand : ICommand;
    }
}
