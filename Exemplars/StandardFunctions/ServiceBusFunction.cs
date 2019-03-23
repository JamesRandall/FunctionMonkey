/*using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace StandardFunctions
{
    public static class ServiceBusFunction
    {
        [FunctionName("ServiceBusFunction")]
        public static void Run([ServiceBusTrigger("myqueue", Connection = "serviceBusConnectionString")]Message myQueueItem, ILogger log)
        {
            string queueItem = System.Text.Encoding.UTF8.GetString(myQueueItem.Body);
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
*/