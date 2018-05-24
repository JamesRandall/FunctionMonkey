using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Commands;

namespace FunctionMonkey.Tests.Integration.Handlers
{
    class StorageQueueCommandHandler : ICommandHandler<StorageQueueCommand>
    {
        public Task ExecuteAsync(StorageQueueCommand command)
        {
            return Task.CompletedTask;
        }
    }
}
