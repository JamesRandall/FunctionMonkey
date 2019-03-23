using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.SignalR
{
    public class SendMessageCommand : ICommand<SignalRMessage>
    {
        public string Message { get; set; }
    }
}
