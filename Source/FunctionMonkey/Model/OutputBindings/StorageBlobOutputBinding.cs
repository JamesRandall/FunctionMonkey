using System.Collections.Generic;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class StorageBlobOutputBinding : AbstractOutputBinding
    {
        public List<StorageBlobOutput> Outputs { get; set; }

        public StorageBlobOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition) : base(associatedFunctionDefinition)
        {
        }
        
        public StorageBlobOutputBinding(string commandResultType) : base(commandResultType)
        {
        }
    }
}