using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpCommandWithNoResultAndValidatorThatFails : ICommand
    {
        public int Value { get; set; }
    }
}
