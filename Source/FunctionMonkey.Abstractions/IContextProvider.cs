using FunctionMonkey.Abstractions.Contexts;

namespace FunctionMonkey.Abstractions
{
    public interface IContextProvider
    {
        ServiceBusContext ServiceBusContext { get; }

        StorageQueueContext StorageQueueContext { get; }

        BlobContext BlobContext { get; }

        EventHubContext EventHubContext { get; }
    }
}
