using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpListQueryParamCommand : ICommand<int>
    {
        public List<int> Value { get; set; }
    }
}