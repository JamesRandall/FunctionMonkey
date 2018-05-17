using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AzureFromTheTrenches.Commanding.AzureFunctions.Infrastructure
{
    internal class JsonSecurityPropertyContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.GetCustomAttributes<SecurityPropertyAttribute>().Any())
            {
                property.Ignored = true;
            }

            return property;
        }
    }
}
