using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Abstractions.Builders
{
    /// <summary>
    /// Allows additional function options and overrides for defaults to be configured 
    /// </summary>
    public interface IFunctionOptions<out TParentBuilder> where TParentBuilder : class
    {
        /// <summary>
        /// Allows an alternative serializer to be provided. If a naming strategy pair is supplied then this
        /// is ignored.
        /// </summary>
        /// <typeparam name="TSerializer">The type of the serializer</typeparam>
        /// <returns></returns>
        TParentBuilder Serializer<TSerializer>()
            where TSerializer : ISerializer;

        /// <summary>
        /// Allows naming strategies for the default ISerializer implementation
        /// </summary>
        /// <typeparam name="TSerializerNamingStrategy"></typeparam>
        /// <typeparam name="TDeserializerNamingStrategy"></typeparam>
        /// <returns></returns>
        TParentBuilder JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>()
            where TSerializerNamingStrategy : NamingStrategy where TDeserializerNamingStrategy : NamingStrategy;
    }
}
