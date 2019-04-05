using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageQueueOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string QueueName { get; set; }

        public StorageQueueOutputBinding(string commandResultItemTypeName, string connectionStringSettingName) : base(commandResultItemTypeName, connectionStringSettingName)
        {
        }
    }
}