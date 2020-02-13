using System;
using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Serialization
{
    public class SecurityPropertyDefaultValueConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType.IsValueType)
            {
                return Activator.CreateInstance(objectType);
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
    
    public class JsonSecurityPropertyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttributes<SecurityPropertyAttribute>().Any() || member.GetCustomAttributes<IgnorePropertyAttribute>().Any())
            {
                property.Ignored = true;
                // when newtonsoft is run on an FSharp record the Ignored property is, well, ignored and the value
                // deserialized / serialized anyway
                property.Converter = new SecurityPropertyDefaultValueConverter();
            }

            return property;
        }
    }
}
