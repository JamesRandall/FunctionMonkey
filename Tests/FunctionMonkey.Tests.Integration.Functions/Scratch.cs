using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Functions
{
    public static class HttpArrayQueryParam2
    {
        [FunctionName("HttpArrayQueryParam2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "GET"
                ,Route = "queryParameters/array2"
                )
            ]
            HttpRequest req,
            ILogger log,
            ExecutionContext executionContext
            
    )
        {
            log.LogInformation("HTTP trigger function HttpArrayQueryParam processed a request.");

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


                var serializer = (FunctionMonkey.Abstractions.ISerializer)
                FunctionMonkey.Runtime.ServiceProvider.GetService(typeof(FunctionMonkey.Serialization.CamelCaseJsonSerializer));

            FunctionMonkey.Tests.Integration.Functions.Commands.HttpArrayQueryParamCommand command;
            string contentType = req.ContentType?.ToLower() ?? "application/json";
            if (contentType.Split(';').Any(x => x.Trim().Equals("application/json", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();
                
                if (!System.String.IsNullOrWhiteSpace(requestBody))
                {
                    command = serializer.Deserialize<FunctionMonkey.Tests.Integration.Functions.Commands.HttpArrayQueryParamCommand>(requestBody);
                }
                else
                {
                    command = new FunctionMonkey.Tests.Integration.Functions.Commands.HttpArrayQueryParamCommand();
                }
            }
            else
            {
                command = new FunctionMonkey.Tests.Integration.Functions.Commands.HttpArrayQueryParamCommand();
            }


            Microsoft.Extensions.Primitives.StringValues queryParameterValues;
            
                    if (req.Query.TryGetValue("Value", out queryParameterValues))
                    {
                        var collection = new System.Int32[queryParameterValues.Count];
                                int queryParameterArrayIndex = 0;
                        foreach(string queryParameterValue in queryParameterValues)
                        {
                            System.Int32 parsedValue = default(System.Int32);
                
                                if (!System.Int32.TryParse(queryParameterValue, out parsedValue))
                                {
                                    return CreateBadParameterResponse("Invalid type for query parameter Value");
                                }
                
                                    collection[queryParameterArrayIndex] = parsedValue;
                                    queryParameterArrayIndex++;
                        }
                
                            command.Value = collection;
                    }
            

    


            var validator = (FunctionMonkey.Abstractions.Validation.IValidator)
                FunctionMonkey.Runtime.ServiceProvider.GetService(typeof(FunctionMonkey.Abstractions.Validation.IValidator));
            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
            {
                    return CreateResponse(400, validationResult, serializer);
            }

            try
            {
                var result = await FunctionMonkey.Runtime.CommandDispatcher.DispatchAsync(command);
                        return CreateResponse(200, result.Result, serializer);
            }
            catch(System.Exception ex)
            {
                    log.LogError(ex, $"Error occurred executing command {command.GetType().Name}");
                    return CreateResponse(500, "Unexpected error", serializer);
            }
        }

        public static IActionResult CreateResponse(int code, object content, FunctionMonkey.Abstractions.ISerializer serializer)
        {
            ContentResult result = new ContentResult();
            result.Content = serializer.Serialize(content, true);
            result.ContentType = "application/json";
            result.StatusCode = code;
            return result;
        }

        public static IActionResult CreateBadParameterResponse(string message)
        {
            ContentResult result = new ContentResult();
            result.Content = message;
            result.ContentType = "text/plain";
            result.StatusCode = 400;
            return result;
        }

        private static string GetRequestUrl(HttpRequest request)
        {
          string str1 = request.Host.Value;
          string str2 = request.PathBase.Value;
          string str3 = request.Path.Value;
          string str4 = request.QueryString.Value;
          return new System.Text.StringBuilder(request.Scheme.Length + "://".Length + str1.Length + str2.Length + str3.Length + str4.Length).Append(request.Scheme).Append("://").Append(str1).Append(str2).Append(str3).Append(str4).ToString();
        }
    
        public delegate System.Boolean TryDelegate<T>(System.String input, out T result);

        private static System.Boolean TryParseNullable<T>(
            System.String input,
            TryDelegate<T> tryFunction,
            out System.Nullable<T> result) where T : struct
        {
            T temp;
            System.Boolean success = tryFunction(input, out temp);
            result = temp;

          return success;
        }
    }    
}
