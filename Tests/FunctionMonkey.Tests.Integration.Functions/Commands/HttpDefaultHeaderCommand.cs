using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpDefaultHeaderCommand : ICommand<SimpleResponse>
    {
        public int DefaultHeaderIntValue { get; set; }
        
        public string DefaultHeaderStringValue { get; set; }
    }
}