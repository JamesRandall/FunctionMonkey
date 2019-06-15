using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpGetQueryParamCommand : ICommand<SimpleResponse>
    {
        public int Value { get; set; }

        public Guid? NullableGuid { get; set; }
    }
}
