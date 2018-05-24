using System;

namespace FunctionMonkey.Model
{
    public class BlobFunctionDefinition : AbstractFunctionDefinition
    {
        public BlobFunctionDefinition(Type commandType) : base("BlobFn", commandType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string BlobPath { get; set; }
    }
}
