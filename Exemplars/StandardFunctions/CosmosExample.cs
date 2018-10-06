using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace StandardFunctions
{
    public static class CosmosExample
    {
        [FunctionName("CosmosExample")]
        public static void Run([CosmosDBTrigger(
            databaseName: "databaseName",
            collectionName: "collectionName",
            ConnectionStringSetting = "cosmosConnectionString",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            foreach (Document document in input)
            {
                document.GetPropertyValue<string>("a");
            }
            
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
