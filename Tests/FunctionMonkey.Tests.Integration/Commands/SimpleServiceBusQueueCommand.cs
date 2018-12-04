using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class SimpleServiceBusQueueCommand : ICommand
    {
        public string SomeValue { get; set; }
    }
}
