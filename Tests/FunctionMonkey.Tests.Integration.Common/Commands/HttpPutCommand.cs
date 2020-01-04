using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpPutCommand : ICommand<SimpleResponse>
    {
        public int Value { get; set; }

        public string Message { get; set; }
    }
}
