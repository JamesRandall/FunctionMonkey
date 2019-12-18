using System;
using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model
{
    public class BlobFunctionDefinition : AbstractFunctionDefinition
    {
        public BlobFunctionDefinition(Type commandType) : base("BlobFn", commandType)
        {
        }
        
        public BlobFunctionDefinition(Type commandType, Type resultType) : base("BlobFn", commandType, resultType)
        {
        }

        public string ConnectionStringName { get; set; }

        public string BlobPath { get; set; }
    }
}
