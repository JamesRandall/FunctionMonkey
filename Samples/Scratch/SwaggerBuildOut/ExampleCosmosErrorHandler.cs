using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FunctionMonkey.Commanding.Cosmos.Abstractions;
using Microsoft.Azure.Documents;
using Microsoft.Build.Framework;
using Microsoft.Extensions.Logging;

namespace SwaggerBuildOut
{
    public class ExampleCosmosErrorHandler : ICosmosDbErrorHandler
    {
        private readonly ILogger<ExampleCosmosErrorHandler> _logger;

        public ExampleCosmosErrorHandler(ILogger<ExampleCosmosErrorHandler> logger)
        {
            _logger = logger;
        }

        public Task<bool> HandleError(Exception ex, Document document)
        {
            _logger.LogError(ex, "Went wrong");
            return Task.FromResult(true);
        }
    }
}
