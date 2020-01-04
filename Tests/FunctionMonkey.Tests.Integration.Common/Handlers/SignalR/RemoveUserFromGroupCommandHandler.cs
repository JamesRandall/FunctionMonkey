using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Abstractions.SignalR;
using FunctionMonkey.Tests.Integration.Common.Commands.SignalR;

namespace FunctionMonkey.Tests.Integration.Common.Handlers.SignalR
{
    internal class RemoveUserFromGroupCommandHandler : ICommandHandler<RemoveUserFromGroupCommand, SignalRGroupAction>
    {
        public Task<SignalRGroupAction> ExecuteAsync(RemoveUserFromGroupCommand command, SignalRGroupAction previousResult)
        {
            return Task.FromResult(new SignalRGroupAction
            {
                Action = GroupActionEnum.Remove,
                GroupName = command.GroupName,
                UserId = command.UserId
            });
        }
    }
}
