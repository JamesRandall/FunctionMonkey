using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace FunctionMonkey.Tests.Integration.Functions.Model
{
    public class MarkerTableEntity : TableEntity
    {
        public static Task<MarkerTableEntity> Success(Guid markerId)
        {
            return Task.FromResult(new MarkerTableEntity
            {
                PartitionKey = markerId.ToString(),
                RowKey = string.Empty
            });
        }
    }
}
