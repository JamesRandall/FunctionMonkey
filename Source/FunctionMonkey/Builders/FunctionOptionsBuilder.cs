using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Abstractions.Builders.Model;
using FunctionMonkey.Model;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Builders
{
    internal class FunctionOptionsBuilder : IFunctionOptionsBuilder
    {
        private readonly AbstractFunctionDefinition _functionDefinition;

        public FunctionOptionsBuilder(AbstractFunctionDefinition functionDefinition)
        {
            _functionDefinition = functionDefinition;
        }
        
        public IFunctionOptionsBuilder Serializer<TSerializer>() where TSerializer : ISerializer
        {
            _functionDefinition.CommandDeserializerType = typeof(TSerializer);
            return this;
        }

        public IFunctionOptionsBuilder JsonNamingStrategies<TDeserializerNamingStrategy, TSerializerNamingStrategy>() where TDeserializerNamingStrategy : NamingStrategy where TSerializerNamingStrategy : NamingStrategy
        {
            _functionDefinition.DeserializerNamingStrategyType = typeof(TDeserializerNamingStrategy);
            _functionDefinition.SerializerNamingStrategyType = typeof(TSerializerNamingStrategy);
            return this;
        }
    }
}