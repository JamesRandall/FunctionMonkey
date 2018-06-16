using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Implementations of this interface are able to deserialize commands from JSON strings while
    /// ensuring that any properties marked with the SecurityPropertyAttribute are not included in
    /// the deserialization process.
    /// </summary>
    public interface ICommandDeserializer
    {
        /// <summary>
        /// Deserializes a command from the provided json
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="json">The JSON</param>
        /// <param name="enforceSecurityProperties">True if SecurityPropertyAttribute behaviour should be applied, defaults to true</param>
        /// <returns>A deserialized command</returns>
        TCommand Deserialize<TCommand>(string json, bool enforceSecurityProperties=true) where TCommand : ICommand;
    }
}
