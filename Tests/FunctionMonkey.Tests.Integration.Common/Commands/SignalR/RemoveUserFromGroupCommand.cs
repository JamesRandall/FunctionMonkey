using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Commands.SignalR
{
    public class RemoveUserFromGroupCommand : ICommand<SignalRGroupAction>
    {
        public string UserId { get; set; }

        public string GroupName { get; set; }
    }
}
