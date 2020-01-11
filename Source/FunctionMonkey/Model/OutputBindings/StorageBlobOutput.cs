using System.IO;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageBlobOutput : AbstractConnectionStringOutputBinding
    {
        
        public string Name { get; set; }
        
        public FileAccess FileAccess { get; set; }

        public StorageBlobOutput(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition, connectionStringSettingName)
        {
        }
        
        public StorageBlobOutput(string commandResultType, string connectionStringSettingName) : base(commandResultType, connectionStringSettingName)
        {
        }
    }
}