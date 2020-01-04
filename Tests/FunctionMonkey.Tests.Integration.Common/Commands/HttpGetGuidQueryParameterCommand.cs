using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpGetGuidQueryParameterCommand : ICommand<Guid>
    {
        public Guid Value { get; set; }
    }
}