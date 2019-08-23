namespace FmFsharpDemo
open Microsoft.Azure.Cosmos
open System

module CosmosDb =
    let cosmosDatabase = "testdatabase"
    let cosmosCollection = "colToDoItems"
    let cosmosConnectionString = Environment.GetEnvironmentVariable("cosmosConnectionString")
    
    let reader<'t> id =
        async {
            use client = new CosmosClient(cosmosConnectionString)
            let container = client.GetContainer(cosmosDatabase, cosmosCollection)
            let! response = container.ReadItemAsync<'t>(id, new PartitionKey(id)) |> Async.AwaitTask
            return response.Resource
        }
    