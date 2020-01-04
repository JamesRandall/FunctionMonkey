namespace FunctionMonkey.Tests.Integration.Common.Handlers.OutputBindings
{
    /*internal class HttpTriggerStorageBlobOutputCommandResultCommandHandler : ICommandHandler<HttpTriggerStorageBlobOutputCommandResultCommand, NamedBlobOutputResponse>
    {
        public Task<NamedBlobOutputResponse> ExecuteAsync(HttpTriggerStorageBlobOutputCommandResultCommand command, NamedBlobOutputResponse previousResult)
        {
            return Task.FromResult(new NamedBlobOutputResponse
            {
                Name = $"{command.MarkerId}.json",
                Value = new BlobOutputResponse
                {
                    MarkerId = command.MarkerId
                }
            });
        }
    }*/
}
