using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure
{
    public class ServiceBusQueuedMarkerIdCommand : ICommand
    {
        public Guid MarkerId { get; set; }

        public static Task<IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand>> SuccessCollection(Guid markerId)
        {
            IReadOnlyCollection<ServiceBusQueuedMarkerIdCommand> response = new[]
            {
                new ServiceBusQueuedMarkerIdCommand
                {
                    MarkerId = markerId
                }
            };
            return Task.FromResult(response);
        }

        public static Task<ServiceBusQueuedMarkerIdCommand> Success(Guid markerId)
        {
            return Task.FromResult(new ServiceBusQueuedMarkerIdCommand
            {
                MarkerId = markerId
            });
        }
    }
}
