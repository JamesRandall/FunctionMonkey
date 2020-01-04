using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class CosmosChangeFeedCommand : ICommand
    {
        public string MarkerId { get; set; }
    }
}
