module FunctionMonkey.FSharp.Tests.Integration.Functions.EntryPoint
open FSharp.Control
open FunctionMonkey.Commanding.Abstractions.Validation
open System
open System.Security.Claims
open System.Threading.Tasks
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Tests.Integration.Functions.HttpClaimFunctions
open Microsoft.WindowsAzure.Storage

exception InvalidTokenException

type HttpCommandWithNoRoute = { nullParam : string option }

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
                         |> Seq.map (fun containerName -> blobClient.GetContainerReference(containerName).CreateIfNotExistsAsync())
                        )
                |> Async.AwaitTask |> Async.Ignore
    }
    
// The C# validator isn't a natural fit for the F# world but it does still exercise the F# validation logic in the
// right way and means compatability is maintained with the acceptance test suite
let private isValidCheck validationResult =
    let castToObjResult = validationResult :> obj
    match castToObjResult with
    | :? ValidationResult as v -> v.IsValid
    | _ -> true
    
let validateToken (bearerToken:string) =
    match bearerToken.Length with
    | 0 -> raise InvalidTokenException
    | _ -> new ClaimsPrincipal(new ClaimsIdentity([
        Claim("claima", "a message")
        Claim("claimb", "42")
    ]))

let app = functionApp {
    outputSourcePath "/Users/jamesrandall/Code/authoredSource"
    defaultAuthorizationMode Anonymous
    openApi "F# Integration Test Functions" "1.0.0"
    openApiUserInterface "/openapi"
    isValid isValidCheck
    tokenValidator validateToken
    claimsMappings [
        claimsMapper.shared("claima", "stringClaim")
        claimsMapper.command("claimb", (fun cmd -> cmd.mappedValue))
    ]
    httpRoute "setup" [
        azureFunction.http (AsyncHandler(createResources), Put)
    ]
    httpRoute "" [
        azureFunction.http (Handler(fun (_:HttpCommandWithNoRoute) -> ()), Get, subRoute="HttpCommandWithNoRoute")
    ]
}
