using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;
using FunctionMonkey.Tests.Integration.Functions.Commands;
using FunctionMonkey.Tests.Integration.Functions.Extensions;

namespace FunctionMonkey.Tests.Integration.Functions.Handlers
{
    internal class CosmosChangeFeedCommandHandler : ICommandHandler<CosmosChangeFeedCommand>
    {
        public async Task ExecuteAsync(CosmosChangeFeedCommand command)
        {
            Guid markerId = Guid.Parse(command.MarkerId);
            await markerId.RecordMarker();
        }
    }
}
