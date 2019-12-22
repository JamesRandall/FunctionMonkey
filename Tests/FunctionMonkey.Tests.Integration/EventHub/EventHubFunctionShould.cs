using System;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Tests.Integration.Common;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Tests.Integration.EventHub
{
    public class EventHubFunctionShould : AbstractIntegrationTest
    {
        [Fact]
        public async Task RespondToEnqueuedItem()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);

            EventHubsConnectionStringBuilder connectionStringBuilder =
                new EventHubsConnectionStringBuilder(Settings.EventHubConnectionString)
                {
                    EntityPath = "maintesthub"
                };
            
            EventHubClient client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            await client.SendAsync(new EventData(Encoding.UTF8.GetBytes(json)));
            
            await marker.Assert();
        }
    }
}