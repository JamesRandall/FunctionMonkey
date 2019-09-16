using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpListQueryStringParamCommand : ICommand<int>
    {
        public List<string> Value { get; set; }
    }
}