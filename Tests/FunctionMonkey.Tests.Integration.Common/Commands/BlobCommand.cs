using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class BlobCommand : ICommand
    {
        public string Name { get; set; }        

        public Guid MarkerId { get; set; }
    }
}
