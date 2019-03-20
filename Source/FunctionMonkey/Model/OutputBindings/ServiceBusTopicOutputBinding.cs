using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    internal class ServiceBusTopicOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string TopicName { get; set; }
    }
}