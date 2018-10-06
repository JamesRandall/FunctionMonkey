using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace SwaggerBuildOut.Commands
{
    public class Name
    {
        [JsonProperty("first")]
        public string First { get; set; }

        [JsonProperty("last")]
        public string Last { get; set; }
    }

    public class CosmosCommand : ICommand
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public Name Name { get; set; }
    }
}
