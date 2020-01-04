using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Commands.SignalR
{
    public class SendMessageCollectionCommand : ICommand<IReadOnlyCollection<SignalRMessage>>
    {
        public IReadOnlyCollection<Guid> MarkerIds { get; set; }

        public string UserId { get; set; }
    }
}
