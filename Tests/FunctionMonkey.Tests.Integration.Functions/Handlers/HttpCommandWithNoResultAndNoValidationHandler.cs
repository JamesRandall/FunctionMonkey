using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class HttpCommandWithNoResultAndNoValidationHandler : ICommandHandler<HttpCommandWithNoResultAndNoValidation>
    {
        public Task ExecuteAsync(HttpCommandWithNoResultAndNoValidation command)
        {
            return Task.CompletedTask;
        }
    }
}
