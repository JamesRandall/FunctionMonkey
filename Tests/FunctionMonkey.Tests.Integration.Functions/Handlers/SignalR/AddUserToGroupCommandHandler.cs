using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;
using FunctionMonkey.Tests.Integration.Functions.Commands.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.SignalR
{
    class AddUserToGroupCommandHandler : ICommandHandler<AddUserToGroupCommand, SignalRGroupAction>
    {
        public Task<SignalRGroupAction> ExecuteAsync(AddUserToGroupCommand command, SignalRGroupAction previousResult)
        {
            return Task.FromResult(new SignalRGroupAction
            {
                Action = GroupActionEnum.Add,
                GroupName = command.GroupName,
                UserId = command.UserId
            });
        }
    }
}
