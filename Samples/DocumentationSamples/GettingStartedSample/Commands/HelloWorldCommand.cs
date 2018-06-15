using AzureFromTheTrenches.Commanding.Abstractions;

namespace GettingStartedSample.Commands
{
    public class HelloWorldCommand : ICommand<string>
    {
        public string Name { get; set; }
    }
}
