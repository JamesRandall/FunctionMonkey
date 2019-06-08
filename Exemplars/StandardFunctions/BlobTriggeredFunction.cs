/*
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace StandardFunctions
{
    public static class BlobTriggeredFunction
    {
        [FunctionName("BlobTriggeredFunction")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "storageConnectionString")]Stream myBlob, string name, BlobProperties blobProperties, ILogger logger)
        {
            //log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string json;
            using (StreamReader reader = new StreamReader(myBlob))
            {
                json = reader.ReadToEnd();
            }
        }
    }
}
*/