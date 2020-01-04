using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpGetRouteParameterCommand : ICommand<RouteParameterResponse>
    {
        public int? OptionalValue { get; set; }

        public int Value { get; set; }

        public string Message { get; set; }

        public string OptionalMessage { get; set; }
    }
}
