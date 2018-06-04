using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace MultiAssemblySample.Commands
{
    public class SimpleCommandWithResult : ICommand<string>
    {
        public bool SomeParameter { get; set; }
    }
}
