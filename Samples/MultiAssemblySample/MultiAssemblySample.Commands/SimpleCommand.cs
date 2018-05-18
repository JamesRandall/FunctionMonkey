using AzureFromTheTrenches.Commanding.Abstractions;

namespace MultiAssemblySample.Commands
{
    public class SimpleCommand : ICommand
    {
        public int SomeValue { get; set; }
    }
}
