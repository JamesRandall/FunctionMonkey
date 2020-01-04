using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    internal class HttpPostWithBytesCommandHandler : ICommandHandler<HttpPostWithBytesCommand, ByteResponse>
    {
        public Task<ByteResponse> ExecuteAsync(HttpPostWithBytesCommand command, ByteResponse previousResult)
        {
            return Task.FromResult(new ByteResponse
            {
                Bytes = command.Bytes
            });
        }
    }
}