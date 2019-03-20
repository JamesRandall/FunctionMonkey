using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    internal class ServiceBusQueueOutputBinding : AbstractOutputBinding
    {
        public string ConnectionStringName { get; set; }

        public string QueueName { get; set; }
    }
}