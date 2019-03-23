using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.SignalR
{
    public class NegotiateCommand : ICommand<SignalRNegotiateResponse>
    {
    }
}
