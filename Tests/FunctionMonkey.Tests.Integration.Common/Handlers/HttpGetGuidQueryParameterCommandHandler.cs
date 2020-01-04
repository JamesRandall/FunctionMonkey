using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands;

namespace FunctionMonkey.Tests.Integration.Common.Handlers
{
    public class HttpGetGuidQueryParameterCommandHandler : ICommandHandler<HttpGetGuidQueryParameterCommand, Guid>
    {
        public Task<Guid> ExecuteAsync(HttpGetGuidQueryParameterCommand command, Guid previousResult)
        {
            return Task.FromResult(command.Value);
        }
    }
}