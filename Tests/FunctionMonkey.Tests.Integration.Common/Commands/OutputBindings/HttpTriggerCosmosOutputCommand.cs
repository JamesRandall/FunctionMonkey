using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands.OutputBindings
{
    public class HttpTriggerCosmosOutputCommand : ICommand<CosmosResponse>
    {
        public Guid MarkerId { get; set; }
    }
}
