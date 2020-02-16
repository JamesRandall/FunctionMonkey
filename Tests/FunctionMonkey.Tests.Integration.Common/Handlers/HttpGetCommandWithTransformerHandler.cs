using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;
using FunctionMonkey.Tests.Integration.Common.Services;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    class HttpGetCommandWithTransformerHandler : ICommandHandler<HttpGetCommandWithTransformer, SimpleResponse>
    {
        public async Task<SimpleResponse> ExecuteAsync(HttpGetCommandWithTransformer command, SimpleResponse previousResult)
        {
            return new SimpleResponse
            {
                Message = command.Message,
                Value = command.Value
            };
        }
    }
}
