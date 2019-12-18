module FunctionMonkey.Tests.FSharp.Integration.Functions.Helpers
open FunctionMonkey.FSharp.Tests.Integration.Functions
open System
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table

let private table =
    CloudStorageAccount
        .Parse(Environment.GetEnvironmentVariable("storageConnectionString"))
        .CreateCloudTableClient()
        .GetTableReference(Constants.Storage.Table.markers)

let createMarker (markerId:Guid) =
    TableEntity(PartitionKey=markerId.ToString(), RowKey=String.Empty)

let recordMarker marker = async {
    let! result = table.ExecuteAsync(TableOperation.InsertOrReplace(marker)) |> Async.AwaitTask
    System.Console.WriteLine(result.HttpStatusCode.ToString())
    ()
}

let recordMarkerFromGuid = createMarker >> recordMarker
