using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageTableOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string TableName { get; set; }

        public StorageTableOutputBinding(string commandResultItemTypeName, string connectionStringSettingName) : base(commandResultItemTypeName, connectionStringSettingName)
        {
        }
    }
}