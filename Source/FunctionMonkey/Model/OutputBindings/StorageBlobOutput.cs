using System.IO;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageBlobOutput : AbstractConnectionStringOutputBinding
    {
        
        public string Name { get; set; }
        
        public FileAccess FileAccess { get; set; }

        public StorageBlobOutput(string commandResultTypeName, string connectionStringSettingName) : base(commandResultTypeName, connectionStringSettingName)
        {
        }
    }
}