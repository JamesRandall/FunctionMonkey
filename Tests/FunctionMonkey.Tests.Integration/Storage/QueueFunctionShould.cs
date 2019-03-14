using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Storage.Helpers;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Storage
{
    public class QueueFunctionShould : AbstractStorageFunctionTest
    {
        [Fact]
        public async Task RespondsToEnqueuedItem()
        {
            var marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);

            CloudQueueClient queueClient = StorageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference("testqueue");
            await queue.AddMessageAsync(new CloudQueueMessage(json));

            await marker.Assert();
        }
    }
}
