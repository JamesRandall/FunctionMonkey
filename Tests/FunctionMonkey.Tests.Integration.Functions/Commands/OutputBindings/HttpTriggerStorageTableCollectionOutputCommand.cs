using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class HttpTriggerStorageTableCollectionOutputCommand : ICommand<IEnumerable<MarkerTableEntity>>
    {
        public Guid MarkerId { get; set; }
    }
}
