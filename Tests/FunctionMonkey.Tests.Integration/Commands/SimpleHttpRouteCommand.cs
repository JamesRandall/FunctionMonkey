using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class SimpleHttpRouteCommand : ICommand
    {
        public string SomeParameter { get; set; }
    }
}
