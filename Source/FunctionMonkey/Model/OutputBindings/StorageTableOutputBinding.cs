using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageTableOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string TableName { get; set; }

        public StorageTableOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition, connectionStringSettingName)
        {
        }
        
        public StorageTableOutputBinding(string commandResultType, string connectionStringSettingName) : base(commandResultType, connectionStringSettingName)
        {
        }
    }
}