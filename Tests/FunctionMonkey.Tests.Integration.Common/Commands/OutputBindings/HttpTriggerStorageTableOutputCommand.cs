using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerStorageTableOutputCommand : ICommand<MarkerTableEntity>
    {
        public Guid MarkerId { get; set; }
    }
}
