using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Abstractions.Builders
{    
    public interface IFunctionOptionsBuilder
    {
        /// <summary>
        /// Allows an alternative serializer to be provided. If a naming strategy pair is supplied then this
        /// is ignored.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the serializer</typeparam>
        /// <returns></returns>
        IFunctionOptionsBuilder Serializer<TSerializer>()
            where TSerializer : ISerializer;

        /// <summary>
        /// Allows naming strategies for the default ISerializer implementation
        /// </summary>
        /// <typeparam name="TSerializerNamingStrategy"></typeparam>
        /// <typeparam name="TDeserializerNamingStrategy"></typeparam>
        /// <returns></returns>
        IFunctionOptionsBuilder JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>()
            where TSerializerNamingStrategy : NamingStrategy where TDeserializerNamingStrategy : NamingStrategy;
        
        /// <summary>
        /// Use this to tell Function Monkey to not attempt to dispatch a command - this can be useful if you
        /// want to, for example, receive a payload on an HTTP API, validate it, then drop it onto a queue
        /// </summary>
        IFunctionOptionsBuilder NoCommandHandler();
    }
}