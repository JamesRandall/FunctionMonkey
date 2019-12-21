using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpStringClaimCommand : ICommand<SimpleResponse>
    {
        public string StringClaim { get; set; }
    }
}