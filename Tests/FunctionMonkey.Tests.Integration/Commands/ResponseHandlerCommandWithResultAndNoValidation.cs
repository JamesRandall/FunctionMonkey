using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class ResponseHandlerCommandWithResultAndNoValidation : ICommand<SimpleResponse>
    {
        public int Value { get; set; }
    }
}
