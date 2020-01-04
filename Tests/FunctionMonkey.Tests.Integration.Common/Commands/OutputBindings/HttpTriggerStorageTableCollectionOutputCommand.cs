using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerStorageTableCollectionOutputCommand : ICommand<IEnumerable<MarkerTableEntity>>
    {
        public Guid MarkerId { get; set; }
    }
}
