using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpIEnumerableQueryParamCommand : ICommand<int>
    {
        public IEnumerable<int> Value { get; set; }
    }
}