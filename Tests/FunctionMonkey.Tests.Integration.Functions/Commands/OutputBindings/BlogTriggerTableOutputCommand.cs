using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class BlogTriggerTableOutputCommand : ICommand<MarkerTableEntity>
    {
        public Guid MarkerId { get; set; }
    }
}
