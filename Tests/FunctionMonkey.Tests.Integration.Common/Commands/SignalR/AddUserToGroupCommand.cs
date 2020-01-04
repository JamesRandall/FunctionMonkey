using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Commands.SignalR
{
    public class AddUserToGroupCommand : ICommand<SignalRGroupAction>
    {
        public string UserId { get; set; }

        public string GroupName { get; set; }
    }
}
