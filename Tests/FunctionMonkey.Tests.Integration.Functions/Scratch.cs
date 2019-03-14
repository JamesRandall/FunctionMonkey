/*
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
    public static class HttpResponseHandlerCommandWithNoResultAndNoValidationScratch
    {
        [FunctionName("HttpResponseHandlerCommandWithNoResultAndNoValidation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function,
                "GET",
                Route = null)]
            HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            log.LogInformation("HTTP trigger function HttpResponseHandlerCommandWithNoResultAndNoValidation processed a request.");

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

            FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseHandlerCommandWithNoResultAndNoValidation command;
            string contentType = req.ContentType?.ToLower() ?? "application/json";
            if (contentType.Split(';').Any(x => x.Trim().Equals("application/json", System.StringComparison.InvariantCultureIgnoreCase)))
            {
                string requestBody = new StreamReader(req.Body).ReadToEnd();

                if (!System.String.IsNullOrWhiteSpace(requestBody))
                {
                    command = serializer.Deserialize<FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseHandlerCommandWithNoResultAndNoValidation>(requestBody);
                }
                else
                {
                    command = new FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseHandlerCommandWithNoResultAndNoValidation();
                }
            }
            else
            {
                command = new FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseHandlerCommandWithNoResultAndNoValidation();
            }


            Microsoft.Extensions.Primitives.StringValues queryParameterValues;
            string method = req.Method.ToUpper();
            if (method == "GET" || method == "DELETE")
            {
                if (req.Query.TryGetValue("Value", out queryParameterValues))
                {
                    System.Int32.TryParse(queryParameterValues[0], out var candidate);
                    command.Value = candidate;
                }
            }




            var responseHandler = (FunctionMonkey.Tests.Integration.Functions.CustomResponseHandler)FunctionMonkey.Runtime.ServiceProvider.GetService(typeof(FunctionMonkey.Tests.Integration.Functions.CustomResponseHandler));

            var validator = (FunctionMonkey.Abstractions.Validation.IValidator)
                FunctionMonkey.Runtime.ServiceProvider.GetService(typeof(FunctionMonkey.Abstractions.Validation.IValidator));
            var validationResult = validator.Validate(command);
            if (!validationResult.IsValid)
            {
                var validatorResponseTask = responseHandler.CreateValidationFailureResponse(command, validationResult);
                var handledValidationResponse = validatorResponseTask != null ? (await validatorResponseTask) : null;
                return handledValidationResponse ?? CreateResponse(400, validationResult, serializer);
            }

            try
            {
                var result = await FunctionMonkey.Runtime.CommandDispatcher.DispatchAsync(command);
                var responseTask = responseHandler.CreateResponse(command);
                var handledResponse = responseTask != null ? (await responseTask) : null;
                return handledResponse ?? new OkResult();
            }
            catch (System.Exception ex)
            {
                var responseTask = responseHandler.CreateResponseFromException(command, ex);
                var handledResponse = responseTask != null ? (await responseTask) : null;
                return handledResponse ?? CreateResponse(500, "Unexpected error", serializer);
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

*/