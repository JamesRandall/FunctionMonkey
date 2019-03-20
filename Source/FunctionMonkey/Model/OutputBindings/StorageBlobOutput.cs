using System.IO;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageBlobOutput : AbstractConnectionStringOutputBinding
    {
        
        public string Name { get; set; }
        
        public FileAccess FileAccess { get; set; }

        public override bool IsReturnType => false;
    }
}