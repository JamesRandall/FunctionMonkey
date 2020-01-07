using System.Linq;
using System.Reflection;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.AspNetCore
{
    public class JsonSecurityPropertyContractResolver : CamelCasePropertyNamesContractResolver
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