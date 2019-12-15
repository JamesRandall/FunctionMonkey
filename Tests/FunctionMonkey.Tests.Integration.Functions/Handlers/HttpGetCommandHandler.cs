using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Services;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpGetCommandHandler : ICommandHandler<HttpGetCommand, SimpleResponse>
    {
        private readonly IMarker _marker;

        public HttpGetCommandHandler(IMarker marker)
        {
            _marker = marker;
        }
        
        public async Task<SimpleResponse> ExecuteAsync(HttpGetCommand command, SimpleResponse previousResult)
        {
            await _marker.RecordMarker(Guid.NewGuid());
            return new SimpleResponse
            {
                Message = command.Message,
                Value = command.Value
            };
        }
    }
}
