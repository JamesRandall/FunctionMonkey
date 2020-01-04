using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpCommandWithNoRoute : ICommand
    {
        public Guid MarkerId { get; set; }
    }
}
