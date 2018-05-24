using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionMonkey.Model
{
    public class BlobStreamFunctionDefinition : AbstractFunctionDefinition
    {
        public BlobStreamFunctionDefinition(Type commandType) : base("BlobFn", commandType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string BlobPath { get; set; }
    }
}
