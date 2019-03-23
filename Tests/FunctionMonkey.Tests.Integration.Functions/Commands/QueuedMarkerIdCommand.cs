using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class QueuedMarkerIdCommand : ICommand
    {
        public Guid MarkerId { get; set; }

        public static Task<IReadOnlyCollection<QueuedMarkerIdCommand>> SuccessCollection(Guid markerId)
        {
            IReadOnlyCollection<QueuedMarkerIdCommand> response = new[]
            {
                new QueuedMarkerIdCommand
                {
                    MarkerId = markerId
                }
            };
            return Task.FromResult(response);
        }

        public static Task<QueuedMarkerIdCommand> Success(Guid markerId)
        {
            return Task.FromResult(new QueuedMarkerIdCommand
            {
                MarkerId = markerId
            });
        }
    }
}
