/*

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

namespace OpenApi.Functions
{
    public static class GetCustomersHttpRequestDto2
    {
        [FunctionName("GetCustomersHttpRequestDto2")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "GET"
                ,Route = "api/v1/Customers"
                )
            ]
            HttpRequest req,
            ILogger log,
            ExecutionContext executionContext
            
    )
        {
            log.LogInformation("HTTP trigger function GetCustomersHttpRequestDto processed a request.");
            
            FunctionMonkey.Runtime.FunctionProvidedLogger.Value = log;
            
            log.LogInformation("HTTP trigger function GetCustomersHttpRequestDto accessed runtime.");
            
            FunctionMonkey.PluginFunctions pluginFunctions = FunctionMonkey.Runtime.PluginFunctions["GetCustomersHttpRequestDto"];

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
            contextSetter.SetHttpContext(null, requestUrl, headerDictionary);

            System.Security.Claims.ClaimsPrincipal principal = null;
            // If we validate tokens then we need to read the header, validate it and retrieve a claims principal. Returning unauthorized if
            // there are any issues


            OpenApi.Customers.GetCustomers.GetCustomersHttpRequestDto command;
            string contentType = req.ContentType?.ToLower() ?? "application/json";
            if (contentType.Split(';').Any(x => x.Trim().Equals("application/json", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                string requestBody = String.Empty;
                using (StreamReader reader = new StreamReader(req.Body))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
                
                if (!System.String.IsNullOrWhiteSpace(requestBody))
                {
                    try
                    {
                        command = (OpenApi.Customers.GetCustomers.GetCustomersHttpRequestDto)pluginFunctions.Deserialize(requestBody, true); // true is to enforce security properties
                    }
                    catch(FunctionMonkey.DeserializationException dex)
                    {
                        if (dex.LineNumber != -1)
                        {
                            System.Text.StringBuilder sbError = new System.Text.StringBuilder("Invalid type in message body at line ");
                            sbError.Append(dex.LineNumber);
                            sbError.Append(" for path ");
                            sbError.Append(dex.Path);
                            return CreateBadParameterResponse(sbError.ToString());
                        }
                        else
                        {
                            return CreateBadParameterResponse(dex.Message);
                        }
                    }
                }
                else
                {
                    command = CreateNewCommand();
                }
            }
            else
            {
                command = CreateNewCommand();
            }



            Microsoft.Extensions.Primitives.StringValues queryParameterValues;
                    if (req.Query.TryGetValue("After", out queryParameterValues))
                    {

                        foreach(string queryParameterValue in queryParameterValues)
                        {
                                System.String parsedValue = default(System.String);
                
                            parsedValue = queryParameterValue;
                
                                    command.After = parsedValue;
                                break;
                        }
                
                    }
                    if (req.Query.TryGetValue("Before", out queryParameterValues))
                    {

                        foreach(string queryParameterValue in queryParameterValues)
                        {
                                System.String parsedValue = default(System.String);
                
                            parsedValue = queryParameterValue;
                
                                    command.Before = parsedValue;
                                break;
                        }
                
                    }
                    if (req.Query.TryGetValue("Limit", out queryParameterValues))
                    {

                        foreach(string queryParameterValue in queryParameterValues)
                        {
                                System.Nullable<System.Int32> parsedValue = default(System.Nullable<System.Int32>);
                
                            {
                                    if (queryParameterValue != null)
                                    {
                                        if(!TryParseNullable(queryParameterValue, System.Int32.TryParse, out parsedValue))
                                        {
                                            return CreateBadParameterResponse("Invalid type for query parameter Limit");
                                        }
                                    }
                            }
                
                                    command.Limit = parsedValue;
                                break;
                        }
                
                    }
                    if (req.Query.TryGetValue("CreatedFrom", out queryParameterValues))
                    {

                        foreach(string queryParameterValue in queryParameterValues)
                        {
                                System.Nullable<System.DateTime> parsedValue = default(System.Nullable<System.DateTime>);
                
                            {
                                    if (queryParameterValue != null)
                                    {
                                        if(!TryParseNullable(queryParameterValue, System.DateTime.TryParse, out parsedValue))
                                        {
                                            return CreateBadParameterResponse("Invalid type for query parameter CreatedFrom");
                                        }
                                    }
                            }
                
                                    command.CreatedFrom = parsedValue;
                                break;
                        }
                
                    }
                    if (req.Query.TryGetValue("CreatedUntil", out queryParameterValues))
                    {

                        foreach(string queryParameterValue in queryParameterValues)
                        {
                                System.Nullable<System.DateTime> parsedValue = default(System.Nullable<System.DateTime>);
                
                            {
                                    if (queryParameterValue != null)
                                    {
                                        if(!TryParseNullable(queryParameterValue, System.DateTime.TryParse, out parsedValue))
                                        {
                                            return CreateBadParameterResponse("Invalid type for query parameter CreatedUntil");
                                        }
                                    }
                            }
                
                                    command.CreatedUntil = parsedValue;
                                break;
                        }
                
                    }
            
                    string headerName;


    

            var validationResult = pluginFunctions.Validate(command);
            if (!pluginFunctions.IsValid(validationResult))
            {
                    return CreateResponse(400, validationResult, pluginFunctions);
            }

            try
            {
                        var result = (await FunctionMonkey.Runtime.CommandDispatcher.DispatchAsync(command)).Result;
                        return CreateResponse(200, result, pluginFunctions);
            }
            catch(System.Exception ex)
            {
                    log.LogError(ex, $"Error occurred executing command {command.GetType().Name}");
                    return CreateResponse(500, "Unexpected error", pluginFunctions);
            }
        }

        public static IActionResult CreateResponse(int code, object content, FunctionMonkey.PluginFunctions pluginFunctions)
        {
            ContentResult result = new ContentResult();
            result.Content = pluginFunctions.Serialize(content, true);
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
                        
        private static OpenApi.Customers.GetCustomers.GetCustomersHttpRequestDto CreateNewCommand()
        {
                return new OpenApi.Customers.GetCustomers.GetCustomersHttpRequestDto();
        }

    }    
}
*/