using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpCommandWithNoRouteHandler : ICommandHandler<HttpCommandWithNoRoute>
    {
        public Task ExecuteAsync(HttpCommandWithNoRoute command)
        {
            return Task.CompletedTask;
        }
    }
}
