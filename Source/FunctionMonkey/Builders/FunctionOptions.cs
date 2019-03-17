using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    internal class FunctionOptions
    {
        private readonly AbstractFunctionDefinition _functionDefinition;

        public FunctionOptions(AbstractFunctionDefinition functionDefinition)
        {
            _functionDefinition = functionDefinition;
        }
        
        public void Serializer<TCommandDeserializer>() where TCommandDeserializer : ISerializer
        {
            _functionDefinition.CommandDeserializerType = typeof(TCommandDeserializer);
        }

        public void JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>()
            where TSerializerNamingStrategy : NamingStrategy where TDeserializerNamingStrategy : NamingStrategy
        {
            _functionDefinition.DeserializerNamingStrategyType = typeof(TDeserializerNamingStrategy);
            _functionDefinition.SerializerNamingStrategyType = typeof(TSerializerNamingStrategy);
        }
    }
}