using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands.HttpResponseShaping
{
    public class HttpResponseHandlerCommandWithResultAndValidatorThatPasses : ICommand<SimpleResponse>
    {
        public int Value { get; set; }
    }
}
