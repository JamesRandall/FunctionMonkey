using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    internal class StorageTableOutputBinding : AbstractConnectionStringOutputBinding
    {
        public string TableName { get; set; }
    }
}