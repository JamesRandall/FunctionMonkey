using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.HttpResponseShaping
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatFails : ICommand<SimpleResponse>
    {
        public int Value { get; set; }
    }
}
