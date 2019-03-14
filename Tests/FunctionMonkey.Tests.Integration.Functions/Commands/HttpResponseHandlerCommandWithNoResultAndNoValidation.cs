using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpResponseHandlerCommandWithNoResultAndNoValidation : ICommand
    {
        public int Value { get; set; }
    }
}
