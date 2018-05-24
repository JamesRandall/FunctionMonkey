using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace StandardFunctions
{
    public static class BlobTriggeredFunction
    {
        [FunctionName("BlobTriggeredFunction")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "storageConnectionString")]Stream myBlob, string name, ILogger logger)
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
