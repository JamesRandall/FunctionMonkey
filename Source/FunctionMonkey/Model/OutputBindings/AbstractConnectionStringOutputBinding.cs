using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public abstract class AbstractConnectionStringOutputBinding : AbstractOutputBinding
    {
        protected AbstractConnectionStringOutputBinding(string commandResultTypeName, string connectionStringSettingName) : base(commandResultTypeName)
        {
            ConnectionStringSettingName = connectionStringSettingName;
        }

        public string ConnectionStringSettingName { get; }
    }
}