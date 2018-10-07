using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Formatting;
using System.Text;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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
            //Lazy<string> errorHandler = new Lazy<string>(() => string.Empty);
            foreach (Document document in input)
            {
                try
                {


                    document.GetPropertyValue<string>("a");
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        document.SaveTo(memoryStream, SerializationFormattingPolicy.None, new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });
                        string json = Encoding.UTF8.GetString(memoryStream.ToArray());
                        string myCommand = JsonConvert.DeserializeObject<string>(json);
                    }
                }
                catch
                {
                    break;
                }
            }
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }
        }
    }
}
