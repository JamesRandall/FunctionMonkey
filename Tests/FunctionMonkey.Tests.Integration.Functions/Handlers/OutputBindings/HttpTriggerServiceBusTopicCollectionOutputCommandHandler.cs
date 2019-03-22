using System.Collections.Generic;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands.Model;
using FunctionMonkey.Tests.Integration.Functions.Commands.OutputBindings;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers.OutputBindings
{
    class HttpTriggerServiceBusTopicCollectionOutputCommandHandler : ICommandHandler<HttpTriggerServiceBusTopicCollectionOutputCommand, IReadOnlyCollection<SimpleResponse>>
    {
        public Task<IReadOnlyCollection<SimpleResponse>> ExecuteAsync(HttpTriggerServiceBusTopicCollectionOutputCommand command, IReadOnlyCollection<SimpleResponse> previousResult)
        {
            return SimpleResponse.SuccessCollection();
        }
    }
}
