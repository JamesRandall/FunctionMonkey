using FunctionMonkey.Abstractions.SignalR;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using SignalRGroupAction = Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRGroupAction;
using SignalRMessage = Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRMessage;

namespace FunctionMonkey.SignalR
{
    public static class Converter
    {
        public static SignalRMessage ToAzureFunctionsObject(
            FunctionMonkey.Abstractions.SignalR.SignalRMessage from)
        {
            return new SignalRMessage
            {
                Arguments = from.Arguments,
                GroupName = from.GroupName,
                Target = from.Target,
                UserId = from.UserId
            };
        }
        
        public static SignalRMessage ToAzureFunctionsObject(SignalRMessage signalRMessage) { return signalRMessage; }

        public static SignalRGroupAction ToAzureFunctionsObject(
            FunctionMonkey.Abstractions.SignalR.SignalRGroupAction from
        )
        {
            return new SignalRGroupAction
            {
                Action = ToAzureFunctionsObject(from.Action),
                GroupName = from.GroupName,
                UserId = from.UserId
            };
        }
        
        public static SignalRGroupAction ToAzureFunctionsObject(SignalRGroupAction groupAction) { return groupAction; }

        public static GroupAction ToAzureFunctionsObject(
            FunctionMonkey.Abstractions.SignalR.GroupActionEnum from
        )
        {
            return from == GroupActionEnum.Add ? GroupAction.Add : GroupAction.Remove;
        }

        public static GroupAction ToAzureFunctionsObject(GroupAction ga)
        {
            return ga;
        }
    }
}
