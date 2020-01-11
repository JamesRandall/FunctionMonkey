using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// Implementations of this interface are able to deserialize commands from JSON strings while
    /// ensuring that any properties marked with the SecurityPropertyAttribute are not included in
    /// the deserialization process.
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Deserializes a command from the provided string
        /// </summary>
        /// <typeparam name="TCommand">The type of the command</typeparam>
        /// <param name="value">The serialized representation</param>
        /// <param name="enforceSecurityProperties">True if SecurityPropertyAttribute behaviour should be applied, defaults to true</param>
        /// <returns>A deserialized command</returns>
        TCommand Deserialize<TCommand>(string value, bool enforceSecurityProperties=true);
        
        /// <summary>
        /// Deserializes a command from the provided string
        /// </summary>
        /// <param name="type">The type of the command</param>
        /// <param name="value">The serialized representation</param>
        /// <param name="enforceSecurityProperties">True if SecurityPropertyAttribute behaviour should be applied, defaults to true</param>
        /// <returns>A deserialized command</returns>
        object Deserialize(Type type, string value, bool enforceSecurityProperties=true);
        
        /// <summary>
        /// Deserializes a command from the provided string
        /// </summary>
        /// <typeparam name="TModel">The model type to serialize</typeparam>
        /// <param name="value">The model</param>
        /// <param name="enforceSecurityProperties">True if SecurityPropertyAttribute behaviour should be applied, defaults to true</param>
        /// <returns>A deserialized command</returns>
        string Serialize<TModel>(TModel value, bool enforceSecurityProperties=true);
    }
}
