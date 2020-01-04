using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Common.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Common.Commands
{
    public class HttpGetGuidRouteParameterCommand : ICommand<GuidPairResponse>
    {
        public Guid RequiredGuid { get; set; }

        public Guid? OptionalGuid { get; set; }
    }
}
