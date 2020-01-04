using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class SimpleServiceBusQueueCommand : ICommand
    {
        public string SomeValue { get; set; }
    }
}
