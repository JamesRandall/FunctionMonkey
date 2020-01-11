using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class ServiceBusTopicOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string TopicName { get; set; }
        public string SessionIdPropertyName { get; set; }
        
        public bool HasSessionId => !string.IsNullOrWhiteSpace(SessionIdPropertyName);

        public ServiceBusTopicOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition, connectionStringSettingName)
        {
        }
        
        public ServiceBusTopicOutputBinding(string commandResultType, string connectionStringSettingName) : base(commandResultType, connectionStringSettingName)
        {
        }
    }
}