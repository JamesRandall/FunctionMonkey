using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class SetupTestResourcesCommandHandler : ICommandHandler<SetupTestResourcesCommand>
    {
        public async Task ExecuteAsync(SetupTestResourcesCommand command)
        {
            // Storage
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageConnectionString"));

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable markerTable = tableClient.GetTableReference(Constants.Storage.Table.Markers);
            await markerTable.CreateIfNotExistsAsync();

            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue testQueue = queueClient.GetQueueReference(Constants.Storage.Queue.TestQueue);
            await testQueue.CreateIfNotExistsAsync();

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer blobCommandsContainer = blobClient.GetContainerReference(Constants.Storage.Blob.BlobCommandContainer);
            await blobCommandsContainer.CreateIfNotExistsAsync();
            CloudBlobContainer streamBlobCommandsContainer = blobClient.GetContainerReference(Constants.Storage.Blob.StreamBlobCommandContainer);
            await streamBlobCommandsContainer.CreateIfNotExistsAsync();

            // Cosmos and Service Bus
            // Created through provisioning
        }
    }
}
