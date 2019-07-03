using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpListQueryParamCommand : ICommand<int>
    {
        public List<int> Value { get; set; }
    }
}