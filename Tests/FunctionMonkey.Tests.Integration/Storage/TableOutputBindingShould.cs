using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Http;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Storage
{
    public class TableOutputBindingShould : AbstractHttpFunctionTest
    {
        [Fact]
        public async Task WriteToStorageTableWhenResponseIsSingular()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("toStorageTable")
                .SetQueryParam("markerId", marker.MarkerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await marker.Assert();
        }

        [Fact]
        public async Task WriteToStorageTableWhenResponseIsCollection()
        {
            MarkerMessage marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("collectionToStorageTable")
                .SetQueryParam("markerId", marker.MarkerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await marker.Assert();
        }
    }
}
