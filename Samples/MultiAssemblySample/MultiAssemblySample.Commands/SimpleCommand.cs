using AzureFromTheTrenches.Commanding.Abstractions;
using MultiAssemblySample.Commands.Abstract;

namespace MultiAssemblySample.Commands
{
    public class SimpleCommand : AbstractCommand, ICommand
    {
        public int SomeValue { get; set; }
    }
}
