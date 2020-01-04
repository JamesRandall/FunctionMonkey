using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpIntClaimCommand : ICommand<SimpleResponse>
    {
        public int MappedValue { get; set; }
    }
}