using System;
using Newtonsoft.Json;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.Model
{
    public class CosmosResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public Guid MarkerId { get; set; }
    }
}
