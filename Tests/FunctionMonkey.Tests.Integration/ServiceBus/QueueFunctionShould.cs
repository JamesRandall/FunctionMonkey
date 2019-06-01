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
    public class QueueFunctionShould : AbstractIntegrationTest
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

            IQueueClient queueClient = new QueueClient(Settings.ServiceBusConnectionString, "testqueue");
            await queueClient.SendAsync(new Message(body));

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

            IQueueClient queueClient = new QueueClient(Settings.ServiceBusConnectionString, "sessionidtestqueue");
            await queueClient.SendAsync(new Message(body) { SessionId = Guid.NewGuid().ToString() });

            await marker.Assert();
        }

        [Fact]
        public async Task OutputToTableBinding()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);
            byte[] body = Encoding.UTF8.GetBytes(json);

            IQueueClient queueClient = new QueueClient(Settings.ServiceBusConnectionString, "tableoutput");
            await queueClient.SendAsync(new Message(body));

            await marker.Assert();
        }
    }
}
