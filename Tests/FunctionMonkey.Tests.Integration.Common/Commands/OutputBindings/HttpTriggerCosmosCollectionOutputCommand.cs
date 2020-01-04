using System;
using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerCosmosCollectionOutputCommand : ICommand<List<CosmosResponse>>
    {
        public Guid MarkerId { get; set; }
    }
}
