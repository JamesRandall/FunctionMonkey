using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatFails : ICommand
    {
        public int Value { get; set; }
    }
}
