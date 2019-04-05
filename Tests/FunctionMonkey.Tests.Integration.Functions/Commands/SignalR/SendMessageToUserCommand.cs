using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.SignalR
{
    public class SendMessageToUserCommand : ICommand<SignalRMessage>
    {
        public string Message { get; set; }
    }
}
