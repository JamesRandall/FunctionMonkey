using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
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