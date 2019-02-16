using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.ChangeFeedProcessor.PartitionManagement;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using System.Data.Common;
using System.Threading;

namespace FunctionApp2
{
    public static class MonitorTrigger
    {
        // Just a normal trigger
        [FunctionName("Trigger")]
        public static void Trigger([CosmosDBTrigger(
            databaseName: "%MonitoredDatabase%",
            collectionName: "%MonitoredCollection%",
            ConnectionStringSetting = "CosmosDB",
            LeaseCollectionPrefix = "%MonitoredDatabaseLeasePrefix%",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)] IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);

                // We add a delay to simulate some work being doing
                Thread.Sleep(1000);
            }
        }

        [FunctionName("Monitor")]
        public static async Task Monitor(
            [TimerTrigger("*/1 * * * * *", RunOnStartup = true)] TimerInfo timer, // Timer will trigger every 1 second, adjust CRON expression
            [CosmosDB("%MonitoredDatabase%", "%MonitoredCollection%", ConnectionStringSetting = "CosmosDB")] DocumentClient monitoredCollectionClient,
            [CosmosDB("%MonitoredDatabase%", "leases", ConnectionStringSetting = "CosmosDB")] DocumentClient leaseCollectionClient,
            ILogger log)
        {
            var estimator = GetRemainingWorkEstimator(monitoredCollectionClient, leaseCollectionClient);
            var remainingWork = await estimator.GetEstimatedRemainingWork();
            // Send custom metric to App Insight
            log.LogInformation(remainingWork.ToString());
            log.LogMetric("RemainingWork", remainingWork);
        }

        // Make it Lazy to reuse between calls
        private static Lazy<IRemainingWorkEstimator> remainingWorkEstimator;

        public static IRemainingWorkEstimator GetRemainingWorkEstimator(
            DocumentClient monitoredCollectionClient,
            DocumentClient leaseCollectionClient)
        {
            if (remainingWorkEstimator == null)
            {
                // Pull the Connection string from the environment, Environment.GetEnvironmentVariable will read the local.settings.json file or the deployed Function App configuration
                CosmosDBConnectionString cosmosDBConnectionString = new CosmosDBConnectionString(Environment.GetEnvironmentVariable("CosmosDB"));
                remainingWorkEstimator = new Lazy<IRemainingWorkEstimator>(() =>
                {
                    var builder = new ChangeFeedProcessorBuilder()
                        .WithHostName("monitor") // Can be a random name
                        .WithProcessorOptions(new ChangeFeedProcessorOptions()
                        {
                            LeasePrefix = Environment.GetEnvironmentVariable("MonitoredDatabaseLeasePrefix")
                        })
                        .WithFeedCollection(new DocumentCollectionInfo()
                        {
                            Uri = cosmosDBConnectionString.ServiceEndpoint,
                            MasterKey = cosmosDBConnectionString.AuthKey,
                            CollectionName = Environment.GetEnvironmentVariable("MonitoredCollection"),
                            DatabaseName = Environment.GetEnvironmentVariable("MonitoredDatabase")
                        })
                        .WithLeaseCollection(new DocumentCollectionInfo()
                        {
                            Uri = cosmosDBConnectionString.ServiceEndpoint,
                            MasterKey = cosmosDBConnectionString.AuthKey,
                            CollectionName = "leases",
                            DatabaseName = Environment.GetEnvironmentVariable("MonitoredDatabase")
                        })
                        .WithFeedDocumentClient(monitoredCollectionClient)
                        .WithLeaseDocumentClient(leaseCollectionClient);

                    return builder.BuildEstimatorAsync().Result;

                });
            }

            return remainingWorkEstimator.Value;
        }

        private class CosmosDBConnectionString
        {
            public CosmosDBConnectionString(string connectionString)
            {
                // Use this generic builder to parse the connection string
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString
                };

                if (builder.TryGetValue("AccountKey", out object key))
                {
                    AuthKey = key.ToString();
                }

                if (builder.TryGetValue("AccountEndpoint", out object uri))
                {
                    ServiceEndpoint = new Uri(uri.ToString());
                }
            }

            public Uri ServiceEndpoint { get; set; }

            public string AuthKey { get; set; }
        }
    }
}