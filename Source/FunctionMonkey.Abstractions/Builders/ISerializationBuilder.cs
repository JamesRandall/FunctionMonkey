namespace FunctionMonkey.Abstractions.Builders
{
    public interface ISerializationBuilder
    {
        /// <summary>
        /// Allows alternative serialization settings to be provided.
        /// </summary>
        /// <typeparam name="TCommandDeserializer"></typeparam>
        /// <returns></returns>
        ISerializationBuilder DefaultCommandDeserializer<TCommandDeserializer>()
            where TCommandDeserializer : ISerializer;
    }
}