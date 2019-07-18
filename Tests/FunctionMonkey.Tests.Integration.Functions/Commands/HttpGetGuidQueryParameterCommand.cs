using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpGetGuidQueryParameterCommand : ICommand<Guid>
    {
        public Guid Value { get; set; }
    }
}