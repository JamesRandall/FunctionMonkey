using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class ResponseHandlerCommandWithNoResultAndNoValidation : ICommand
    {
        public int Value { get; set; }
    }
}
