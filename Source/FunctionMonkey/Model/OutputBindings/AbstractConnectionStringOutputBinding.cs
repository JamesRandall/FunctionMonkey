using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public abstract class AbstractConnectionStringOutputBinding : AbstractOutputBinding
    {
        protected AbstractConnectionStringOutputBinding(string commandResultItemTypeName, string connectionStringSettingName) : base(commandResultItemTypeName)
        {
            ConnectionStringSettingName = connectionStringSettingName;
        }

        public string ConnectionStringSettingName { get; }
    }
}