using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.SignalR
{
    public class AddUserToGroupCommand : ICommand<SignalRGroupAction>
    {
        public string UserId { get; set; }

        public string GroupName { get; set; }
    }
}
