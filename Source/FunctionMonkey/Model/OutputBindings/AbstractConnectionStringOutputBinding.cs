using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public abstract class AbstractConnectionStringOutputBinding : AbstractOutputBinding
    {
        protected AbstractConnectionStringOutputBinding(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition)
        {
            ConnectionStringSettingName = connectionStringSettingName;
        }
        
        protected AbstractConnectionStringOutputBinding(string commandResultType, string connectionStringSettingName) : base(commandResultType)
        {
            ConnectionStringSettingName = connectionStringSettingName;
        }

        public string ConnectionStringSettingName { get; set; }
    }
}