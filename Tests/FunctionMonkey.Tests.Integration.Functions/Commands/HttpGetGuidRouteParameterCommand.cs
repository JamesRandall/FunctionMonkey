using System;
using System.Collections.Generic;
using System.Text;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands
{
    public class HttpGetGuidRouteParameterCommand : ICommand<GuidPairResponse>
    {
        public Guid RequiredGuid { get; set; }

        public Guid? OptionalGuid { get; set; }
    }
}
