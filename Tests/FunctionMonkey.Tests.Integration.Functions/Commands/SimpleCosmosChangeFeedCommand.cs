using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class SimpleCosmosChangeFeedCommand : ICommand
    {
        public string Value { get; set; }
    }
}
