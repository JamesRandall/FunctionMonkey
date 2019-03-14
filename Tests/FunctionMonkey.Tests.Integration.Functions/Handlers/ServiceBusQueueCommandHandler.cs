using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class ServiceBusQueueCommandHandler : ICommandHandler<ServiceBusQueueCommand>
    {
        public async Task ExecuteAsync(ServiceBusQueueCommand command)
        {
            await command.MarkerId.RecordMarker();
        }
    }
}
