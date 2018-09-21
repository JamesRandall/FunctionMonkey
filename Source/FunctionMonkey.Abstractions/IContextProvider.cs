using FunctionMonkey.Abstractions.Contexts;

namespace FunctionMonkey.Abstractions
{
    /// <summary>
    /// This can be injected into command handlers to obtain additional context information from the trigger - for example how many times a message
    /// has been dequeued.
    /// 
    /// Care should be taken not to overuse this as it effectively binds the handler to an explicit host / message type implementation and ideally these
    /// concerns are kept separate.
    /// </summary>
    public interface IContextProvider
    {
        ServiceBusContext ServiceBusContext { get; }

        StorageQueueContext StorageQueueContext { get; }

        BlobContext BlobContext { get; }

        EventHubContext EventHubContext { get; }

        ExecutionContext ExecutionContext { get; }

        HttpContext HttpContext { get; }
    }
}
