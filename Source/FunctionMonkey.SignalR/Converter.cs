using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace FunctionMonkey.SignalR
{
    public static class Converter
    {
        public static Microsoft.Azure.WebJobs.Extensions.SignalRService.SignalRMessage ToAzureFunctionsObject(
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
    }
}
