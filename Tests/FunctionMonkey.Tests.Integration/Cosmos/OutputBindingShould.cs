using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Cosmos
{
    public class OutputBindingShould : AbstractHttpFunctionTest
    {
        private const string Database = "testdatabase";

        private const string Collection = "testcollection";

        [Fact]
        public async Task WriteToDatabaseWhenResponseIsSingular()
        {
            Guid markerId = Guid.NewGuid();

            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("toCosmos")
                .SetQueryParam("markerId", markerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await WaitForMarkerInDatabase(markerId);
        }

        [Fact]
        public async Task WriteToDatabaseWhenResponseIsCollection()
        {
            Guid markerId = Guid.NewGuid();

            HttpResponseMessage response = await Settings.Host
                .AppendPathSegment("outputBindings")
                .AppendPathSegment("collectionToCosmos")
                .SetQueryParam("markerId", markerId)
                .GetAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            await WaitForMarkerInDatabase(markerId);
        }

        private async Task WaitForMarkerInDatabase(Guid markerId)
        {
            const int delayIncrement = 750;
            const int maximumDelay = delayIncrement * 20;

            string cosmosConnectionString = Settings.CosmosConnectionString;
            string[] cosmosConnectionStringParts = cosmosConnectionString.Split(';');
            string cosmosEndpoint = cosmosConnectionStringParts[0].Substring("AccountEndpoint=".Length);
            string cosmosAuthKey = cosmosConnectionStringParts[1].Substring("AccountKey=".Length).TrimEnd(';');
            CosmosMarker marker = null;
            using (DocumentClient documentClient = new DocumentClient(new Uri(cosmosEndpoint), cosmosAuthKey))
            {
                int totalDelay = 0;
                do
                {
                    await Task.Delay(delayIncrement);
                    totalDelay += delayIncrement;

                    try
                    {
                        marker = await documentClient.ReadDocumentAsync<CosmosMarker>(UriFactory.CreateDocumentUri(Database, Collection,
                            markerId.ToString()), new RequestOptions { PartitionKey = new PartitionKey(markerId.ToString()) });
                    }
                    catch (DocumentClientException ex)
                    {
                        if (ex.StatusCode != HttpStatusCode.NotFound)
                        {
                            throw;
                        }
                    }                    
                } while (totalDelay < maximumDelay && marker == null);
            }

            Assert.NotNull(marker);
        }
    }
}
