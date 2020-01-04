using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping
{
    public class HttpCommandWithNoResultAndValidatorThatPasses : ICommand
    {
        public int Value { get; set; }
    }
}
