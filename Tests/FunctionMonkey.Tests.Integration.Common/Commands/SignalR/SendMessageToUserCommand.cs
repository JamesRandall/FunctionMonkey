using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Commands.SignalR
{
    public class SendMessageToUserCommand : ICommand<SignalRMessage>
    {
        public string Message { get; set; }
    }
}
