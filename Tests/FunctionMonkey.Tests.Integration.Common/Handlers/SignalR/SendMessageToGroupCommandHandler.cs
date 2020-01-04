using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;
using FunctionMonkey.Tests.Integration.Common.Commands.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.SignalR
{
    internal class SendMessageToGroupCommandHandler : ICommandHandler<SendMessageToGroupCommand, SignalRMessage>
    {
        public Task<SignalRMessage> ExecuteAsync(SendMessageToGroupCommand command, SignalRMessage previousResult)
        {
            return Task.FromResult(new SignalRMessage
            {
                Arguments = new object[] {command.MarkerId},
                GroupName = command.GroupName,
                Target = "sendMessageToGroupCommand"
            });
        }
    }
}
