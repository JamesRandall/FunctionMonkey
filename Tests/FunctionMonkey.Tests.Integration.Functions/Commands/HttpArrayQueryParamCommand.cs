using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpArrayQueryParamCommand : ICommand<int>
    {
        public int[] Value { get; set; }
    }
}