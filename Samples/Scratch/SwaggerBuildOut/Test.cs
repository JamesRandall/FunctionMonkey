
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SwaggerBuildOut
{
    public static class SwaggerUi
    {
        private static readonly Dictionary<string, string> SwaggerContentTypes = new Dictionary<string, string>
        {
            { ".html", "text/html" },
            {".png", "image/png" },
            {".map", "application/json" },
            {".css", "text/css"},
            {".js","application/javascript" }
        };

        [FunctionName("Test")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger logger)
        {
            if (req.Method == "GET")
            {

            }

            string name = req.Query["name"];
            
            using (Stream stream = typeof(SwaggerUi).Assembly.GetManifestResourceStream($"SwaggerBuildOut.Swagger.{name}"))
            {
                if (stream != null)
                {
                    ContentResult result = new ContentResult();
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        result.Content = reader.ReadToEnd();
                    }

                    if (!SwaggerContentTypes.TryGetValue(Path.GetExtension(name).ToLower(), out var contentType))
                    {
                        contentType = "text/plain";
                    }

                    result.ContentType = contentType;

                    return result;
                }
            }
            return new NotFoundResult();
        }
    }
}
