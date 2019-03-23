using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.SignalR;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.SignalR
{
    internal class NegotiateCommandHandler : ICommandHandler<NegotiateCommand, SignalRNegotiateResponse>
    {
        public Task<SignalRNegotiateResponse> ExecuteAsync(NegotiateCommand command, SignalRNegotiateResponse previousResult)
        {
            return Task.FromResult(new SignalRNegotiateResponse
            {
                HubName = Constants.SignalR.HubName,
                UserId = "1234"
            });
        }
    }
}
