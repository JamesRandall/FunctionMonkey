using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Commanding.Abstractions.Validation;
using FunctionMonkey.Model;
using FunctionMonkey.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace FunctionMonkey.Testing.Implementation
{
    internal class HttpDispatcher
    {
        private readonly ICommandDispatcher _dispatcher;
        private readonly IServiceProvider _serviceProvider;

        public HttpDispatcher(ICommandDispatcher dispatcher, IServiceProvider serviceProvider)
        {
            _dispatcher = dispatcher;
            _serviceProvider = serviceProvider;
        }

        public async Task<IActionResult> DispatchAndConvertToActionResult<TResult>(
            ICommand<TResult> command,
            IHttpResponseHandler httpResponseHandler,
            HttpFunctionDefinition httpFunctionDefinition)
        {
            IActionResult actionResult = null;
            try
            {
                TResult result = await _dispatcher.DispatchAsync(command);
                // TODO: Handle validation here
                Task<IActionResult> responseHandlerTask = httpResponseHandler.CreateResponse(command, result);
                if (responseHandlerTask != null)
                {
                    actionResult = await responseHandlerTask;
                }

                if (actionResult == null)
                {
                    actionResult = CreateResponse(200, result, httpFunctionDefinition);
                }
            }
            catch (ValidationException vex)
            {
                actionResult = await CreateValidationResponse(command, vex.ValidationResult, httpFunctionDefinition, httpResponseHandler);
            }
            catch (Exception ex)
            {
                Task exceptionResponseHandlerTask = httpResponseHandler.CreateResponseFromException(command, ex);
                if (exceptionResponseHandlerTask != null)
                {
                    actionResult = await httpResponseHandler.CreateResponseFromException(command, ex);
                }

                if (actionResult == null)
                {
                    actionResult = CreateResponse(500, "Unexpected error", httpFunctionDefinition);
                }
            }

            return actionResult;
        }

        public async Task<IActionResult> DispatchAndConvertToActionResult(ICommand command, IHttpResponseHandler httpResponseHandler,
            HttpFunctionDefinition httpFunctionDefinition)
        {
            IActionResult actionResult = null;
            try
            {
                await _dispatcher.DispatchAsync(command);
                // TODO: Handle validation here
                Task<IActionResult> responseHandlerTask = httpResponseHandler.CreateResponse(command);
                if (responseHandlerTask != null)
                {
                    actionResult = await responseHandlerTask;
                }

                if (actionResult == null)
                {
                    actionResult = new OkResult();
                }
            }
            catch (ValidationException vex)
            {
                actionResult = await CreateValidationResponse(command, vex.ValidationResult, httpFunctionDefinition, httpResponseHandler);
            }
            catch (Exception ex)
            {
                Task exceptionResponseHandlerTask = httpResponseHandler.CreateResponseFromException(command, ex);
                if (exceptionResponseHandlerTask != null)
                {
                    actionResult = await httpResponseHandler.CreateResponseFromException(command, ex);
                }

                if (actionResult == null)
                {
                    actionResult = CreateResponse(500, "Unexpected error", httpFunctionDefinition);
                }
            }

            return actionResult;
        }

        private async Task<IActionResult> CreateValidationResponse(ICommand command, ValidationResult validationResult, HttpFunctionDefinition httpFunctionDefinition, IHttpResponseHandler responseHandler)
        {
            IActionResult actionResult = null;
            Task<IActionResult> validationResponseHandlerTask = responseHandler.CreateValidationFailureResponse(command, validationResult);
            if (validationResponseHandlerTask != null)
            {
                actionResult = await validationResponseHandlerTask;
            }

            return actionResult ?? (CreateResponse(400, validationResult, httpFunctionDefinition));
        }

        private IActionResult CreateResponse(int code, object content, HttpFunctionDefinition httpFunctionDefinition)
        {
            ISerializer serializer = CreateSerializer(httpFunctionDefinition);
            ContentResult result = new ContentResult
            {
                Content = serializer.Serialize(content), ContentType = "application/json", StatusCode = code
            };
            return result;
        }

        private ISerializer CreateSerializer(HttpFunctionDefinition httpFunctionDefinition)
        {
            if (httpFunctionDefinition.SerializerNamingStrategyType != null)
            {
                NamingStrategy serializerNamingStrategy = (NamingStrategy)Activator.CreateInstance(httpFunctionDefinition.SerializerNamingStrategyType);
                NamingStrategy deserializerNamingStrategy = (NamingStrategy)Activator.CreateInstance(httpFunctionDefinition.DeserializerNamingStrategyType);
                ISerializer serializer = new NamingStrategyJsonSerializer(deserializerNamingStrategy, serializerNamingStrategy);
                return serializer;
            }

            return (ISerializer)_serviceProvider.GetRequiredService(httpFunctionDefinition.CommandDeserializerType);
        }
    }
}
