
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace StandardFunctions
{
    public static class HttpTriggeredFunction
    {
        class SomeResult
        {
            public string Message { get; set; }
        }

        [FunctionName("HttpTriggeredFunction")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req,
            TraceWriter log,
            ExecutionContext executionContext)
        {
            log.Info("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? CreateResponse(HttpStatusCode.OK, new SomeResult
                {
                    Message = "Hello world"
                })
                : CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body");
        }

        public static IActionResult CreateResponse(HttpStatusCode code, object content)
        {
            ContentResult result = new ContentResult();
            result.Content = JsonConvert.SerializeObject(content, Formatting.Indented,
                new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()});
            result.ContentType = "application/json";
            result.StatusCode = (int) code;
            return result;
        }
    }
}
