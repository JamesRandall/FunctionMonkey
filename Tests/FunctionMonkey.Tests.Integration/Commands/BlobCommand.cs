using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class BlobCommand : ICommand
    {
        public string SomeProperty { get; set; }
    }
}
