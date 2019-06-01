using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Tests.Integration.Common;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Tests.Integration.ServiceBus
{
    public class SubscriptionFunctionShould : AbstractIntegrationTest
    {
        [Fact]
        public async Task RespondToEnqueuedItem()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);
            byte[] body = Encoding.UTF8.GetBytes(json);

            ITopicClient topicClient = new TopicClient(Settings.ServiceBusConnectionString, "testtopic");
            await topicClient.SendAsync(new Message(body));

            await marker.Assert();
        }

        [Fact]
        public async Task RespondToEnqueuedItemWithSessionId()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);
            byte[] body = Encoding.UTF8.GetBytes(json);

            ITopicClient topicClient = new TopicClient(Settings.ServiceBusConnectionString, "sessionidtesttopic");
            await topicClient.SendAsync(new Message(body) { SessionId = Guid.NewGuid().ToString()});

            await marker.Assert();
        }
    }
}
