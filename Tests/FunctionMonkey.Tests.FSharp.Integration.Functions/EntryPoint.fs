module FunctionMonkey.Tests.FSharp.Integration.Functions.EntryPoint
open FSharp.Control
open System
open System.Threading.Tasks
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open Microsoft.WindowsAzure.Storage

let private createResources () =
    let storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("storageConnectionString"))
    let tableClient = storageAccount.CreateCloudTableClient()
    let queueClient = storageAccount.CreateCloudQueueClient()
    let blobClient = storageAccount.CreateCloudBlobClient()
    let queues = [
        Constants.Storage.Queue.markerQueue
        Constants.Storage.Queue.testQueue
    ]
    let containers = [
        Constants.Storage.Blob.blobCommandContainer
        Constants.Storage.Blob.outputBlobContainer
        Constants.Storage.Blob.blobOutputCommandContainer
        Constants.Storage.Blob.streamBlobCommandContainer
    ]
    async {
        do! tableClient.GetTableReference(Constants.Storage.Table.markers).CreateIfNotExistsAsync() |> Async.AwaitTask |> Async.Ignore
        do! Task.WhenAll(queues
                         |> Seq.map (fun queueName -> queueClient.GetQueueReference(queueName).CreateIfNotExistsAsync())
                        )
                |> Async.AwaitTask |> Async.Ignore
        do! Task.WhenAll(containers
                         |> Seq.map (fun queueName -> blobClient.GetContainerReference(queueName).CreateIfNotExistsAsync())
                        )
                |> Async.AwaitTask |> Async.Ignore
    }

let app = functionApp {
    outputSourcePath "/Users/jamesrandall/code/authoredSource"
    defaultAuthorizationMode Anonymous
    openApi "F# Integration Test Functions" "1.0.0"
    openApiUserInterface "/openapi"
    
    httpRoute "setup" [
        azureFunction.http (AsyncHandler(createResources), Put)
    ]
}
