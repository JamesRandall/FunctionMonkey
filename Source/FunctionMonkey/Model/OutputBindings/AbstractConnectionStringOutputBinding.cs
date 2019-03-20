using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public abstract class AbstractConnectionStringOutputBinding : AbstractOutputBinding
    {
        public string ConnectionStringSettingName { get; set; }
    }
}