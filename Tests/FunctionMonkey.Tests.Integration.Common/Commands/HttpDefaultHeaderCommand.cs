using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpDefaultHeaderCommand : ICommand<SimpleResponse>
    {
        public int DefaultHeaderIntValue { get; set; }
        
        public string DefaultHeaderStringValue { get; set; }
    }
}