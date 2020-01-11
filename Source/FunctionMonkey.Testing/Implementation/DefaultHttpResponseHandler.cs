using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Commanding.Abstractions.Validation;
using Microsoft.AspNetCore.Mvc;

namespace FunctionMonkey.Testing.Implementation
{
    internal class DefaultHttpResponseHandler : IHttpResponseHandler
    {
        public Task<IActionResult> CreateResponseFromException<TCommand>(TCommand command, Exception ex)
        {
            return null;
        }

        // We can't use where TCommand : ICommand<TResult> due to the function injection - we can't bind the generic return type
        // (will revisit and check)

        public Task<IActionResult> CreateResponse<TCommand, TResult>(TCommand command, TResult result)
        {
            return null;
        }

        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
        {
            return null;
        }

        public Task<IActionResult> CreateValidationFailureResponse<TCommand>(TCommand command, ValidationResult validationResult)
        {
            return null;
        }
    }
}
