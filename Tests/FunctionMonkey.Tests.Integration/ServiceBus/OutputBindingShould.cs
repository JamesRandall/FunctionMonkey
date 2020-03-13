using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Http;
using Xunit;

namespace FunctionMonkey.Tests.Integration.ServiceBus
{
    // Note that the output binding service bus functions write to a queue that is listened to by another
    // function in the integration tests that listens to this queue. This then sets the marker in the table.
    public class OutputBindingShould : AbstractHttpFunctionTest
    {
        [Fact]
        public async Task WriteToServiceBusQueueWhenResponseIsSingular()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("toServiceBusQueue")
                .SetQueryParam("markerId", marker.MarkerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await marker.Assert();
        }
        
        [Fact]
        public async Task WriteToServiceBusQueueWhenResponseIsSingularAndOutputConverterRuns()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("toServiceBusQueueWithConverter")
                .SetQueryParam("markerId", marker.MarkerId)
                .SetQueryParam("value", 42)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            marker.Value++; // the converter adds 1 to the value
            await marker.Assert();
        }

        [Fact]
        public async Task WriteToServiceBusQueueWhenResponseIsCollection()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("collectionToServiceBusQueue")
                .SetQueryParam("markerId", marker.MarkerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await marker.Assert();
        }

        [Fact]
        public async Task WriteToServiceBusTopicWhenResponseIsSingular()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("toServiceBusTopic")
                .SetQueryParam("markerId", marker.MarkerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await marker.Assert();
        }

        [Fact]
        public async Task WriteToServiceBusTopicWhenResponseIsCollection()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("collectionToServiceBusTopic")
                .SetQueryParam("markerId", marker.MarkerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await marker.Assert();
        }
    }
}
