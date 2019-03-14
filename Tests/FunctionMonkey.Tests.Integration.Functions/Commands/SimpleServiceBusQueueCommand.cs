using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class SimpleServiceBusQueueCommand : ICommand
    {
        public string SomeValue { get; set; }
    }
}
