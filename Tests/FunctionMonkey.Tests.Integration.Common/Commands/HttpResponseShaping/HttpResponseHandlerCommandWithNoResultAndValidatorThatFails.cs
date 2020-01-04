using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping
{
    public class HttpResponseHandlerCommandWithNoResultAndValidatorThatFails : ICommand
    {
        public int Value { get; set; }
    }
}
