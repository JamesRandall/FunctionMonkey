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

type ServiceBusSubscriptionCommand =
    {
        markerId: Guid
    }
    
type ServiceBusQueueTriggerTableOutputCommand =
    {
        markerId: Guid
    }
    
let private serviceBusQueue (command:ServiceBusQueueCommand) = 
    command.markerId |> recordMarkerFromGuid
    
let private serviceBusSubscription (command:ServiceBusSubscriptionCommand) =
    command.markerId |> recordMarkerFromGuid
    
let private serviceBusQueueTriggerTableOutput (command:ServiceBusQueueTriggerTableOutputCommand) =
    command.markerId |> createMarker
    
let serviceBusFunctions = functions {
    serviceBus DefaultConnectionStringSettingName [
        azureFunction.serviceBusQueue(AsyncHandler(serviceBusQueue), Constants.ServiceBus.queue)
        azureFunction.serviceBusSubscription(AsyncHandler(serviceBusSubscription), Constants.ServiceBus.topicName, Constants.ServiceBus.subscriptionName)
        azureFunction.serviceBusQueue(Handler(serviceBusQueueTriggerTableOutput), Constants.ServiceBus.tableOutputQueue)
            |> storageTable Constants.Storage.Table.markers
    ]
}

