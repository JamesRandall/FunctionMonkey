using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpPostWithBytesCommand : ICommand<ByteResponse>
    {
        public byte[] Bytes { get; set; }
    }
}
