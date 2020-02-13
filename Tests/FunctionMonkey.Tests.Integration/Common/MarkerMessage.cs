using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionMonkey.Tests.Integration.Common
{
    public class MarkerMessage
    {
        public Guid MarkerId { get; set; }
        
        public int? Value { get; set; }

        public async Task Assert()
        {
            const int delayIncrement = 750;
            const int maximumDelay = delayIncrement * 120;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AbstractIntegrationTest.Settings.StorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable markerTable = tableClient.GetTableReference("markers");

            int totalDelay = 0;
            MarkerTableEntity marker = null;
            do
            {
                await Task.Delay(delayIncrement);
                totalDelay += delayIncrement;
                marker = (MarkerTableEntity)(await markerTable.ExecuteAsync(TableOperation.Retrieve<MarkerTableEntity>(MarkerId.ToString(), string.Empty)))?.Result;
            } while (totalDelay < maximumDelay && marker == null);

            Xunit.Assert.NotNull(marker);
            Xunit.Assert.Equal(Value, marker.Value);
        }
    }
}
