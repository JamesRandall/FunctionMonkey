using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class StorageQueueCommand : ICommand
    {
        public Guid MarkerId { get; set; }
    }
}
