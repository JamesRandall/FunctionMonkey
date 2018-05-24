using AzureFromTheTrenches.Commanding.Abstractions;

namespace FunctionMonkey.Tests.Integration.Commands
{
    public class StorageQueueCommand : ICommand
    {
        public string Message { get; set; }
    }
}
