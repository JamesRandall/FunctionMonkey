using AzureFromTheTrenches.Commanding.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpCommandWithNoRoute : ICommand
    {
        public Guid MarkerId { get; set; }
    }
}
