using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;
using FunctionMonkey.Tests.Integration.Functions.Commands.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.SignalR
{
    class SendMessageCollectionCommandHandler : ICommandHandler<SendMessageCollectionCommand, IReadOnlyCollection<SignalRMessage>>
    {
        public Task<IReadOnlyCollection<SignalRMessage>> ExecuteAsync(SendMessageCollectionCommand command, IReadOnlyCollection<SignalRMessage> previousResult)
        {
            IReadOnlyCollection<SignalRMessage> messages = command.MarkerIds.Select(x => new SignalRMessage
            {
                Arguments = new object[] {x},
                GroupName = null,
                Target = "sendMessageCollectionCommand",
                UserId = command.UserId
            }).ToArray();

            return Task.FromResult(messages);
        }
    }
}
