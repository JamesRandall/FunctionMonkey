using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public abstract class AbstractServiceBusOutputBinding : AbstractConnectionStringOutputBinding
    {
        public AbstractServiceBusOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition, connectionStringSettingName)
        {
        }
        
        public AbstractServiceBusOutputBinding(string commandResultType, string connectionStringSettingName) : base(commandResultType, connectionStringSettingName)
        {
        }
        
        public string SessionIdPropertyName { get; set; }
        
        public bool HasSessionId => !string.IsNullOrWhiteSpace(SessionIdPropertyName);
    }
}