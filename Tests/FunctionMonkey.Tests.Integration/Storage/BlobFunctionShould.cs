using System;
using System.Threading.Tasks;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Storage.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Storage
{
    public class BlobFunctionShould : AbstractStorageFunctionTest
    {
        [Fact]
        public async Task RespondToUploadedBlob()
        {
            var marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);

            CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("blobcommands");

            CloudBlockBlob blob = container.GetBlockBlobReference($"{marker.MarkerId}.json");
            await blob.UploadTextAsync(json);

            await marker.Assert();
        }

        [Fact]
        public async Task OutputToTableBinding()
        {
            var marker = new MarkerMessage
            {
                MarkerId = Guid.NewGuid()
            };
            string json = JsonConvert.SerializeObject(marker);

            CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("outputbindingcontainer");

            CloudBlockBlob blob = container.GetBlockBlobReference($"{marker.MarkerId}.json");
            await blob.UploadTextAsync(json);

            await marker.Assert();
        }
    }
}
