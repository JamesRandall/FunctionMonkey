using System.Collections.Generic;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageBlobOutputBinding : AbstractOutputBinding
    {
        public List<StorageBlobOutput> Outputs { get; set; }

        public StorageBlobOutputBinding(string commandResultTypeName) : base(commandResultTypeName)
        {
        }
    }
}