using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class CosmosTriggerTableOutputCommand : ICommand<MarkerTableEntity>
    {
        public Guid MarkerId { get; set; }
    }
}
