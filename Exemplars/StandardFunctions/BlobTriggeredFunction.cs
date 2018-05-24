using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace StandardFunctions
{
    public static class BlobTriggeredFunction
    {
        [FunctionName("BlobTriggeredFunction")]
        public static void Run([BlobTrigger("samples-workitems/{name}", Connection = "myStorageAccount")]Stream myBlob, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string json;
            using (StreamReader reader = new StreamReader(myBlob))
            {
                json = reader.ReadToEnd();
            }
        }
    }
}
