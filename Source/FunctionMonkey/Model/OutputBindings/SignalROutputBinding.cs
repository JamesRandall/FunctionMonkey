namespace FunctionMonkey.Model.OutputBindings
{
    public class SignalROutputBinding : AbstractConnectionStringOutputBinding
    {
        public SignalROutputBinding(string commandResultItemTypeName, string connectionStringSettingName) : base(commandResultItemTypeName, connectionStringSettingName)
        {
        }

        public string HubName { get; set; }

        public string SignalROutputTypeName { get; set; }
    }
}
