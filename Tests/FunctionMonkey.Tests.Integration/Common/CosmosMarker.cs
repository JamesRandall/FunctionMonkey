using System;
using Newtonsoft.Json;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class CosmosMarker
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }

        public Guid MarkerId { get; set; }
    }
}
