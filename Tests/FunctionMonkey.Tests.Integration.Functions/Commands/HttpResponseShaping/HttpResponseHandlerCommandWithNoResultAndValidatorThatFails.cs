using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatFails : ICommand
    {
        public int Value { get; set; }
    }
}
