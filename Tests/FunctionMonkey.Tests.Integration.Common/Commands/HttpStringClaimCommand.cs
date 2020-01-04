using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpStringClaimCommand : ICommand<SimpleResponse>
    {
        public string StringClaim { get; set; }
    }
}