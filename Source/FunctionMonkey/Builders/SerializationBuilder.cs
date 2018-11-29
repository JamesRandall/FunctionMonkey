using System;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Builders;
using FunctionMonkey.Infrastructure;
using FunctionMonkey.Serialization;

namespace FunctionMonkey.Builders
{
    public class SerializationBuilder : ISerializationBuilder
    {
        public Type DefaultCommandDeserializerType { get; private set; } = typeof(CamelCaseJsonSerializer);
    
        public ISerializationBuilder DefaultCommandDeserializer<TCommandDeserializer>() where TCommandDeserializer : ISerializer
        {
            DefaultCommandDeserializerType = typeof(TCommandDeserializer);
            return this;
        }
    }
}