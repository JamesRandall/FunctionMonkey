using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Commands.SignalR
{
    public class SendMessageCommand : ICommand<SignalRMessage>
    {
        public string Message { get; set; }
    }
}
