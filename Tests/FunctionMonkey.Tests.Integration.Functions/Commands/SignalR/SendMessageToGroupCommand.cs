using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.SignalR
{
    public class SendMessageToGroupCommand : ICommand<SignalRMessage>
    {
        public Guid MarkerId { get; set; }

        public string GroupName { get; set; }
    }
}
