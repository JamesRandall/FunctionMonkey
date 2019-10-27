module FunctionMonkey.Tests.FSharp.Integration.Functions.ServiceBusFunctions
open FunctionMonkey.FSharp
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.OutputBindings
open FunctionMonkey.FSharp.Tests.Integration.Functions
open FunctionMonkey.Tests.FSharp.Integration.Functions.Helpers
open System

type ServiceBusQueueCommand =
    {
        markerId: Guid
    }
    
type ServiceBusQueuedMarkerIdCommand =
    {
        markerId: Guid
    }

type ServiceBusSubscriptionCommand =
    {
        markerId: Guid
    }
    
type ServiceBusQueueTriggerTableOutputCommand =
    {
        markerId: Guid
    }
    
type ServiceBusSessionIdQueueCommand =
    {
        sessionId: Guid
        markerId: Guid
    }
    
type ServiceBusSessionIdSubscriptionCommand =
    {
        sessionId: Guid
        markerId: Guid
    }
    
let private serviceBusQueue (command:ServiceBusQueueCommand) = 
    command.markerId |> recordMarkerFromGuid
    
let private serviceBusSubscription (command:ServiceBusSubscriptionCommand) =
    command.markerId |> recordMarkerFromGuid
    
let private serviceBusQueueTriggerTableOutput (command:ServiceBusQueueTriggerTableOutputCommand) =
    command.markerId |> createMarker
    
let private serviceBusQueuedMarkerId (command:ServiceBusQueuedMarkerIdCommand) =
    command.markerId |> recordMarkerFromGuid
    
let private serviceBusSessionIdQueue (command:ServiceBusSessionIdQueueCommand) =
    command.markerId |> recordMarkerFromGuid
    
let private serviceBusSessionIdSubscription (command:ServiceBusSessionIdSubscriptionCommand) =
    command.markerId |> recordMarkerFromGuid
    
let serviceBusFunctions = functions {
    serviceBus DefaultConnectionStringSettingName [
        azureFunction.serviceBusQueue(AsyncHandler(serviceBusQueue), Constants.ServiceBus.queue)
        azureFunction.serviceBusSubscription(AsyncHandler(serviceBusSubscription), Constants.ServiceBus.topicName, Constants.ServiceBus.subscriptionName)
        azureFunction.serviceBusQueue(Handler(serviceBusQueueTriggerTableOutput), Constants.ServiceBus.tableOutputQueue)
            |> storageTable Constants.Storage.Table.markers
        azureFunction.serviceBusQueue(AsyncHandler(serviceBusSessionIdQueue), Constants.ServiceBus.sessionIdQueue, sessionIdEnabled=true)
        azureFunction.serviceBusSubscription(AsyncHandler(serviceBusSessionIdSubscription),
                                             Constants.ServiceBus.sessionIdTopicName,
                                             Constants.ServiceBus.sessionIdSubscriptionName,
                                             sessionIdEnabled=true)
            
        // not used directly by tests but used to prove outputbindings
        azureFunction.serviceBusQueue(AsyncHandler(serviceBusQueuedMarkerId), Constants.ServiceBus.markerQueue)
    ]
}

