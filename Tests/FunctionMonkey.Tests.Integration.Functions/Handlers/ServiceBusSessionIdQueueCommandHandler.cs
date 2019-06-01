using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class ServiceBusSessionIdQueueCommandHandler : ICommandHandler<ServiceBusSessionIdQueueCommand>
    {
        public async Task ExecuteAsync(ServiceBusSessionIdQueueCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
