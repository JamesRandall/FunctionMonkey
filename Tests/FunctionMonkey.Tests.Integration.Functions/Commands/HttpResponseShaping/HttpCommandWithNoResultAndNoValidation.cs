using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping
{
    public class HttpCommandWithNoResultAndNoValidation : ICommand
    {
        public int Value { get; set; }
    }
}
