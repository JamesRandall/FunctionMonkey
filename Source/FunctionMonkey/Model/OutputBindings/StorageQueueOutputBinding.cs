using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    internal class StorageQueueOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string QueueName { get; set; }
    }
}