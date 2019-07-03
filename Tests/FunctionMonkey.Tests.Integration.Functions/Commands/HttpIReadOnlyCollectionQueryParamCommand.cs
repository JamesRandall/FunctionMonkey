using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpIReadOnlyCollectionQueryParamCommand : ICommand<int>
    {
        public IReadOnlyCollection<int> Value { get; set; }
    }
}