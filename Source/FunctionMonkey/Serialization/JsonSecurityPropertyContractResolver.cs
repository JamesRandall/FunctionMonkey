using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Serialization
{
    public class JsonSecurityPropertyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttributes<SecurityPropertyAttribute>().Any() || member.GetCustomAttributes<IgnorePropertyAttribute>().Any())
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
