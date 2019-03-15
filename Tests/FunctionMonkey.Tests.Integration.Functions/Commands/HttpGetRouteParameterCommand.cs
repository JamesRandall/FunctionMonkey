using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpGetRouteParameterCommand : ICommand<RouteParameterResponse>
    {
        public int? OptionalValue { get; set; }

        public int Value { get; set; }

        public string Message { get; set; }

        public string OptionalMessage { get; set; }
    }
}
