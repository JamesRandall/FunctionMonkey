using System;
using System.Threading.Tasks;
using FunctionMonkey.Tests.Integration.Common;
using Microsoft.Azure.Documents.Client;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Cosmos
{
    public class ChangeFeedFunctionShould : AbstractIntegrationTest
    {
        [Fact]
        public async Task RespondToNewItem()
        {
            string cosmosConnectionString = Settings.CosmosConnectionString;
            string[] cosmosConnectionStringParts = cosmosConnectionString.Split(';');
            string cosmosEndpoint = cosmosConnectionStringParts[0].Substring("AccountEndpoint=".Length);
            string cosmosAuthKey = cosmosConnectionStringParts[1].Substring("AccountKey=".Length).TrimEnd(';');
            using (DocumentClient documentClient = new DocumentClient(new Uri(cosmosEndpoint), cosmosAuthKey))
            {
                MarkerMessage marker = new MarkerMessage
                {
                    MarkerId = Guid.NewGuid()
                };
                await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("testdatabase", "testcollection"), marker);

                await marker.Assert();
            }
        }

        [Fact]
        public async Task OutputToTableBinding()
        {
            string cosmosConnectionString = Settings.CosmosConnectionString;
            string[] cosmosConnectionStringParts = cosmosConnectionString.Split(';');
            string cosmosEndpoint = cosmosConnectionStringParts[0].Substring("AccountEndpoint=".Length);
            string cosmosAuthKey = cosmosConnectionStringParts[1].Substring("AccountKey=".Length).TrimEnd(';');
            using (DocumentClient documentClient = new DocumentClient(new Uri(cosmosEndpoint), cosmosAuthKey))
            {
                MarkerMessage marker = new MarkerMessage
                {
                    MarkerId = Guid.NewGuid()
                };
                await documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri("testdatabase", "outputtablecollection"), marker);

                await marker.Assert();
            }
        }
    }
}
