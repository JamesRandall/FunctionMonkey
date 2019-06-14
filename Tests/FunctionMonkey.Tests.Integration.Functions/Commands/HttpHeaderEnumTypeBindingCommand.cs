using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpHeaderEnumTypeBindingCommand : ICommand<SimpleResponse>
    {
        public enum BindingTestEnum
        {
            SomeValue = 56,
            AnotherValue = 92316
        }
        
        public BindingTestEnum Value { get; set; }
    }
}
