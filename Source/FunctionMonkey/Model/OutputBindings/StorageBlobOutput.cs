using System.IO;

namespace FunctionMonkey.Model.OutputBindings
{
    internal class StorageBlobOutput : AbstractConnectionStringOutputBinding
    {
        
        public string Name { get; set; }
        
        public FileAccess FileAccess { get; set; }
    }
}