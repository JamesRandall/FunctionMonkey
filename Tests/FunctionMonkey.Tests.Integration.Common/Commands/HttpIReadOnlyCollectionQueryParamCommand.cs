using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpIReadOnlyCollectionQueryParamCommand : ICommand<int>
    {
        public IReadOnlyCollection<int> Value { get; set; }
    }
}