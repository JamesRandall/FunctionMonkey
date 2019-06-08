using System;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.TestInfrastructure;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class HttpTriggerServiceBusTopicWithSessionIdOutputCommand : ICommandWithNoHandler
    {
        public Guid MarkerId { get; set; }
        
        public Guid ASessionId { get; set; }
    }
}