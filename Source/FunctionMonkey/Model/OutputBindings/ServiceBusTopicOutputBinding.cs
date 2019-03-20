using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    internal class ServiceBusTopicOutputBinding : AbstractOutputBinding
    {
        public string ConnectionStringName { get; set; }

        public string TopicName { get; set; }
    }
}