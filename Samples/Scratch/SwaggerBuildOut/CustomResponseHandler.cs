using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Http.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace SwaggerBuildOut
{
    public class CustomResponseHandler : IHttpResponseHandler
    {
        public Task<IActionResult> CreateResponse<TCommand>(TCommand command, Exception ex) where TCommand : ICommand
        {
            return null;
        }

        public Task<IActionResult> CreateResponse<TCommand, TResult>(TCommand command, TResult result) where TCommand : ICommand<TResult>
        {
            ContentResult response = new ContentResult();
            response.Content = "I've customised the response";
            response.StatusCode = 999;
            return Task.FromResult((IActionResult)response);
        }

        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
        {
            return null;
        }
    }
}
