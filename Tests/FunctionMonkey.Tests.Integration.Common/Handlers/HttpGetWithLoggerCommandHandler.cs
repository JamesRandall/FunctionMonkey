using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using Microsoft.Extensions.Logging;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class HttpGetWithLoggerCommandHandler : ICommandHandler<HttpGetWithLoggerCommand>
    {
        private readonly ILogger _logger;

        public HttpGetWithLoggerCommandHandler(ILogger logger)
        {
            _logger = logger;
        }
        
        public Task ExecuteAsync(HttpGetWithLoggerCommand command)
        {
            _logger.LogWarning("Logger should be called");
            return Task.CompletedTask;
        }
    }
}