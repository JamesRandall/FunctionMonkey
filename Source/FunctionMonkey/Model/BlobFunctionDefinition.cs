using System;
using FunctionMonkey.Abstractions.Builders.Model;

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
