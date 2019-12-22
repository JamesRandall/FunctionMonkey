using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class EventHubOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string EventHub { get; set; }
        
        public EventHubOutputBinding(string commandResultItemTypeName, string connectionStringSettingName) : base(commandResultItemTypeName, connectionStringSettingName)
        {
        }
    }
}