using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatPasses : ICommand
    {
        public int Value { get; set; }
    }
}
