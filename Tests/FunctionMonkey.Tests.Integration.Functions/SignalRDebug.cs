/*using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FunctionMonkey.Abstractions.Builders.Model;
using System.Security.Claims;

namespace FunctionMonkey.Tests.Integration.Functions.Functions
{
    public static class SignalRDebug
    {
        [FunctionName("SignalRDebug")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "GET",
                Route = "simpleNegotiate")                
            ]
            HttpRequest req,
            ILogger log,
            ExecutionContext executionContext,
            [SignalRConnectionInfo(HubName="simplehub", ConnectionStringSetting="signalRConnectionString", UserId="{headers.x-ms-client-principal-id}")]SignalRConnectionInfo connectionInfo
        )
        {
            log.LogInformation("HTTP trigger function SignalRBindingExpressionNegotiate processed a request.");

            FunctionMonkey.Runtime.FunctionProvidedLogger.Value = log;

            string requestUrl = GetRequestUrl(req);
            var contextSetter = (FunctionMonkey.Abstractions.IContextSetter)
                FunctionMonkey.Runtime.ServiceProvider.GetService(typeof(FunctionMonkey.Abstractions.IContextSetter));
            contextSetter.SetExecutionContext(executionContext.FunctionDirectory,
                executionContext.FunctionAppDirectory,
                executionContext.FunctionName,
                executionContext.InvocationId);
            var headerDictionary = new Dictionary<string, IReadOnlyCollection<string>>();
            foreach (var headerKeyValuesPair in req.Headers)
            {
                string[] values = headerKeyValuesPair.Value.ToArray();
                headerDictionary.Add(headerKeyValuesPair.Key, values);
            }
            contextSetter.SetHttpContext(requestUrl, headerDictionary);

            System.Security.Claims.ClaimsPrincipal principal = null;
            // If we validate tokens then we need to read the header, validate it and retrieve a claims principal. Returning unauthorized if
            // there are any issues


            return new OkObjectResult(connectionInfo);
        }

        private static string GetRequestUrl(HttpRequest request)
        {
          string str1 = request.Host.Value;
          string str2 = request.PathBase.Value;
          string str3 = request.Path.Value;
          string str4 = request.QueryString.Value;
          return new System.Text.StringBuilder(request.Scheme.Length + "://".Length + str1.Length + str2.Length + str3.Length + str4.Length).Append(request.Scheme).Append("://").Append(str1).Append(str2).Append(str3).Append(str4).ToString();
        }
    }    
}
*/
