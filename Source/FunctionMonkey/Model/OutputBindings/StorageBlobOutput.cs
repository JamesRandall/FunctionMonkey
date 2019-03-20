using System.IO;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageBlobOutput
    {
        public string ConnectionStringSettingName { get; set; }
        
        public string Name { get; set; }
        
        public FileAccess FileAccess { get; set; }
    }
}