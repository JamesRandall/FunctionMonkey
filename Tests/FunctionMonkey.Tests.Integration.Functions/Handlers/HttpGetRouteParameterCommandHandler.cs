using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpGetRouteParameterCommandHandler : ICommandHandler<HttpGetRouteParameterCommand, RouteParameterResponse>
    {
        public Task<RouteParameterResponse> ExecuteAsync(HttpGetRouteParameterCommand command, RouteParameterResponse previousResult)
        {
            return Task.FromResult(new RouteParameterResponse
            {
                OptionalMessage = command.OptionalMessage,
                Message = command.Message,
                Value = command.Value,
                OptionalValue = command.OptionalValue
            });
        }
    }
}
