using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.Http;
using FunctionMonkey.Commanding.Abstractions.Validation;
using Microsoft.AspNetCore.Mvc;

namespace FmAspNetCore
{
    public class StringContentResponseHandler : IHttpResponseHandler
    {
        public Task<IActionResult> CreateResponseFromException<TCommand>(TCommand command, Exception ex) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> CreateResponse<TCommand, TResult>(TCommand command, TResult result) where TCommand : ICommand
        {
            string stringResult = result.ToString();
            IActionResult contentResult = new ContentResult()
            {
                Content = stringResult,
                ContentType = "text/plain",
                StatusCode = 200
            };
            return Task.FromResult(contentResult);
        }

        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> CreateValidationFailureResponse<TCommand>(TCommand command, ValidationResult validationResult) where TCommand : ICommand
        {
            throw new NotImplementedException();
        }
    }
}