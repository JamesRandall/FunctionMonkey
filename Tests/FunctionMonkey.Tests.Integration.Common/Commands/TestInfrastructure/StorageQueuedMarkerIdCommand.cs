using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.TestInfrastructure
{
    public class StorageQueuedMarkerIdCommand : ICommand
    {
        public Guid MarkerId { get; set; }

        public static Task<IReadOnlyCollection<StorageQueuedMarkerIdCommand>> SuccessCollection(Guid markerId)
        {
            IReadOnlyCollection<StorageQueuedMarkerIdCommand> response = new[]
            {
                new StorageQueuedMarkerIdCommand
                {
                    MarkerId = markerId
                }
            };
            return Task.FromResult(response);
        }

        public static Task<StorageQueuedMarkerIdCommand> Success(Guid markerId)
        {
            return Task.FromResult(new StorageQueuedMarkerIdCommand
            {
                MarkerId = markerId
            });
        }
    }
}
