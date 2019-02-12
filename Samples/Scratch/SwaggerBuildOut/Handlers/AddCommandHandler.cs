using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.Logging;
using SwaggerBuildOut.Commands;
using SwaggerBuildOut.Commands.Responses;
using SwaggerBuildOut.Services;

namespace SwaggerBuildOut.Handlers
{
    internal class AddCommandHandler : ICommandHandler<AddCommand, AddResult>
    {
        private readonly IMessageProvider _messageProvider;
        private readonly ILogger _logger;

        public AddCommandHandler(IMessageProvider messageProvider, ILogger logger)
        {
            _messageProvider = messageProvider;
            _logger = logger;
        }

        public Task<AddResult> ExecuteAsync(AddCommand command, AddResult previousResult)
        {
            _logger.Log(LogLevel.Error, _messageProvider.GetMessage());
            return Task.FromResult(new AddResult
            {
                ValueResultThing = command.ValueOne + command.ValueTwo
            });
        }
    }
}
