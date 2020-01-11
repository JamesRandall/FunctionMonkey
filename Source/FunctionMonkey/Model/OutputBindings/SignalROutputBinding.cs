using FunctionMonkey.Abstractions.Builders.Model;

namespace FunctionMonkey.Model.OutputBindings
{
    public class SignalROutputBinding : AbstractConnectionStringOutputBinding
    {
        public SignalROutputBinding(AbstractFunctionDefinition associatedFunctionDefinition, string connectionStringSettingName) : base(associatedFunctionDefinition, connectionStringSettingName)
        {
        }
        
        public SignalROutputBinding(string commandResultType, string connectionStringSettingName) : base(commandResultType, connectionStringSettingName)
        {
        }

        public string HubName { get; set; }

        public string SignalROutputTypeName { get; set; }

        public const string SignalROutputMessageType =
            "Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRMessage";

        public const string SignalROutputGroupActionType =
            "Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRGroupAction";
    }
}
