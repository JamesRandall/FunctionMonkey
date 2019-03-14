using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class CommandWithNoResultAndNoValidation : ICommand
    {
        public int Value { get; set; }
    }
}
