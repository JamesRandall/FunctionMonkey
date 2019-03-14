using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatPasses : ICommand<SimpleResponse>
    {
        public int Value { get; set; }
    }
}
