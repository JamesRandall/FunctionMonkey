using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class HttpGetGuidRouteParameterCommandHandler : ICommandHandler<HttpGetGuidRouteParameterCommand, GuidPairResponse>
    {
        public Task<GuidPairResponse> ExecuteAsync(HttpGetGuidRouteParameterCommand command, GuidPairResponse previousResult)
        {
            return Task.FromResult(new GuidPairResponse
            {
                ValueOne = command.RequiredGuid,
                ValueTwo = command.OptionalGuid
            });
        }
    }
}
