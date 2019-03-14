using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpCommandWithNoResultAndNoValidation : ICommand
    {
        public int Value { get; set; }
    }
}
