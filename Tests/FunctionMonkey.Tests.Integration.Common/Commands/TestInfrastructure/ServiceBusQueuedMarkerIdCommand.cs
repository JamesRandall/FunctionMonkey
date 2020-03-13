using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure
{
    public class ServiceBusQueuedMarkerIdCommand : IOptionalValueCommand, ICommand
    {
        public Guid MarkerId { get; set; }
        
        public int? Value { get; set; }

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

        public static Task<ServiceBusQueuedMarkerIdCommand> Success(Guid markerId, int? value= null)
        {
            return Task.FromResult(new ServiceBusQueuedMarkerIdCommand
            {
                MarkerId = markerId,
                Value = value
            });
        }
    }
}
