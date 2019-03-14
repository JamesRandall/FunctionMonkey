using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class CommandWithResultAndNoValidation : ICommand<SimpleResponse>
    {
        public int Value { get; set; }
    }
}
