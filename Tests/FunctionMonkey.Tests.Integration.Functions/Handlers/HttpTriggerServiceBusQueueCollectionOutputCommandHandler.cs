using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    class HttpTriggerServiceBusQueueCollectionOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusQueueCollectionOutputCommand, IReadOnlyCollection<SimpleResponse>>
    {
        public Task<IReadOnlyCollection<SimpleResponse>> ExecuteAsync(HttpTriggerServiceBusQueueCollectionOutputCommand command, IReadOnlyCollection<SimpleResponse> previousResult)
        {
            return SimpleResponse.SuccessCollection();
        }
    }
}
