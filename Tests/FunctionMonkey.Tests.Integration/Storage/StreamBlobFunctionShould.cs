using System;
using System.Threading.Tasks;
using FunctionMonkey.Tests.Integration.Common;
using FunctionMonkey.Tests.Integration.Storage.Helpers;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using Xunit;

namespace FunctionMonkey.Tests.Integration.Storage
{
    public class StreamBlobFunctionShould : AbstractStorageFunctionTest
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
            CloudBlobContainer container = blobClient.GetContainerReference("streamblobcommands");

            CloudBlockBlob blob = container.GetBlockBlobReference($"{marker.MarkerId}.json");
            await blob.UploadTextAsync(json);

            await marker.Assert();
        }
    }
}
