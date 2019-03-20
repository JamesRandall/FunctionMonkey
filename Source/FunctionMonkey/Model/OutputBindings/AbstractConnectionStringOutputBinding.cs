using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    internal abstract class AbstractConnectionStringOutputBinding : AbstractOutputBinding
    {
        public string ConnectionStringSettingName { get; set; }
    }
}