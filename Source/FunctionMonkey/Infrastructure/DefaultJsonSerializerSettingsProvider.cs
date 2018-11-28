using FunctionMonkey.Abstractions;
using Newtonsoft.Json;

namespace FunctionMonkey.Infrastructure
{
    internal class DefaultJsonSerializerSettingsProvider : IJsonSerializerSettingsProvider
    {
        public JsonSerializerSettings Get()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new JsonSecurityPropertyContractResolver()
            };
        }
    }
}