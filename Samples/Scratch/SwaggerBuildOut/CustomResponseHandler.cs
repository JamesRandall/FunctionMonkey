using System;
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
            if (result == null)
            {
                return Task.FromResult((IActionResult)new NoContentResult());
            }

            return null;
        }

        public Task<IActionResult> CreateResponse<TCommand>(TCommand command)
        {
            return null;
        }
    }
}
