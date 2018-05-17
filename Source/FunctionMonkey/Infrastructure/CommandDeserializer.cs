using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using Newtonsoft.Json;

namespace FunctionMonkey.Infrastructure
{
    internal class CommandDeserializer : ICommandDeserializer
    {
        private readonly JsonSerializerSettings _serializerSettings;

        public CommandDeserializer()
        {
            _serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new JsonSecurityPropertyContractResolver()
            };
        }

        public TCommand Deserialize<TCommand>(string json, bool enforceSecurityProperties) where TCommand : ICommand
        {
            if (enforceSecurityProperties)
            {
                return JsonConvert.DeserializeObject<TCommand>(json, _serializerSettings);
            }
            else
            {
                return JsonConvert.DeserializeObject<TCommand>(json);
            }
        }
    }
}
