using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure
{
    public class ServiceBusSubscriptionMarkerIdCommand : ICommand
    {
        public Guid MarkerId { get; set; }

        public static Task<IReadOnlyCollection<ServiceBusSubscriptionMarkerIdCommand>> SuccessCollection(Guid markerId)
        {
            IReadOnlyCollection<ServiceBusSubscriptionMarkerIdCommand> response = new[]
            {
                new ServiceBusSubscriptionMarkerIdCommand
                {
                    MarkerId = markerId
                }
            };
            return Task.FromResult(response);
        }

        public static Task<ServiceBusSubscriptionMarkerIdCommand> Success(Guid markerId)
        {
            return Task.FromResult(new ServiceBusSubscriptionMarkerIdCommand
            {
                MarkerId = markerId
            });
        }
    }
}
