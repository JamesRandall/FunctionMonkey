using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class CosmosChangeFeedCommand : ICommand
    {
        public string MarkerId { get; set; }
    }
}
