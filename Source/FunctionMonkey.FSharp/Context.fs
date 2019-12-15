namespace FunctionMonkey.FSharp
open FunctionMonkey.Abstractions.Contexts

module Context =
    type FunctionExecutionContext =
        | Http of HttpContext
        | Blob of BlobContext
        | EventHub of EventHubContext
        | ServiceBus of ServiceBusContext
        | StorageQueue of StorageQueueContext
        | NoContextAvailable
    
    let functionContext () =
        if not (FunctionMonkey.Infrastructure.ContextManager.HttpContextLocal.Value = null) then
            Http(FunctionMonkey.Infrastructure.ContextManager.HttpContextLocal.Value)
        elif not (FunctionMonkey.Infrastructure.ContextManager.BlobContextLocal.Value = null) then
            Blob(FunctionMonkey.Infrastructure.ContextManager.BlobContextLocal.Value)
        elif not (FunctionMonkey.Infrastructure.ContextManager.EventHubContextLocal = null) then
            EventHub(FunctionMonkey.Infrastructure.ContextManager.EventHubContextLocal.Value)
        elif not (FunctionMonkey.Infrastructure.ContextManager.ServiceBusContextLocal = null) then
            ServiceBus(FunctionMonkey.Infrastructure.ContextManager.ServiceBusContextLocal.Value)
        elif not (FunctionMonkey.Infrastructure.ContextManager.StorageQueueContextLocal = null) then
            StorageQueue(FunctionMonkey.Infrastructure.ContextManager.StorageQueueContextLocal.Value)
        else
            NoContextAvailable
            
    let executionContext () =
        FunctionMonkey.Infrastructure.ContextManager.ExecutionContextLocal.Value
        

