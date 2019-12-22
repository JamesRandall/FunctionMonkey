using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure
{
    public class EventHubQueuedMarkerIdCommand : ICommand
    {
        public Guid MarkerId { get; set; }

        public static Task<EventHubQueuedMarkerIdCommand> Success(Guid markerId)
        {
            return Task.FromResult(new EventHubQueuedMarkerIdCommand
            {
                MarkerId = markerId
            });
        }
    }
}
