using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpPostCommandWithSecurityProperty : ICommand<SimpleResponse>
    {
        [SecurityProperty]
        public int Value { get; set; }

        public string Message { get; set; }
    }
}