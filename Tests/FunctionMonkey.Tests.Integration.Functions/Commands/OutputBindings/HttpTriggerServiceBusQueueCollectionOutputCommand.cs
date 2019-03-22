using System.Collections.Generic;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings
{
    public class HttpTriggerServiceBusQueueCollectionOutputCommand : ICommand<IReadOnlyCollection<SimpleResponse>>
    {
        
    }
}