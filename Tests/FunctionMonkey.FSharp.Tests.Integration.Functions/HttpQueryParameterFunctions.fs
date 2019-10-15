module FunctionMonkey.Tests.FSharp.Integration.Functions.HttpQueryParameterFunctions
open System
open System.Collections.Generic
open FunctionMonkey.FSharp.Models
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Tests.Integration.Functions.CommonModel

type HttpGetQueryParamCommand =
    {
        value: int
        nullableGuid: System.Nullable<Guid>
    }
    
type HttpGetGuidQueryParameterCommand =
    {
        value: Guid
    }
    
type HttpArrayQueryParamCommand =
    {
        value: int array
    }
    
type HttpIReadOnlyCollectionQueryParamCommand =
    {
        value: IReadOnlyCollection<int>
    }
    
type HttpListQueryParamCommand =
    {
        value: int list
    }
    
type HttpListQueryStringParamCommand =
    {
        value: string list
    }
    
type HttpIEnumerableQueryParamCommand =
    {
        value: IEnumerable<int>
    }
    
let private queryParam (command : HttpGetQueryParamCommand) : SimpleResponse =
    {
        message = (sprintf "%O" command.nullableGuid)
        value = command.value
    }
    
let private guidQueryParameter (command : HttpGetGuidQueryParameterCommand) : Guid =
    command.value
    
let private arrayQueryParam (command : HttpArrayQueryParamCommand) : int =
    command.value |> Seq.sum
    
let private readonlyCollectionQueryParam (command : HttpIReadOnlyCollectionQueryParamCommand) : int =
    command.value |> Seq.sum
    
let private listQueryParam (command : HttpListQueryParamCommand) : int =
    command.value |> Seq.sum
    
let private listQueryStringParam (command : HttpListQueryStringParamCommand) : int =
    command.value |> Seq.map(fun v -> System.Int32.Parse(v)) |> Seq.sum
    
let private enumerableQueryParam (command : HttpIEnumerableQueryParamCommand) : int =
    command.value |> Seq.sum

let httpVerbFunctions = functions {
    httpRoute "queryParameters" [
        azureFunction.http (Handler(queryParam), Get)
        azureFunction.http (Handler(guidQueryParameter), Get, subRoute="/guidQueryParam")
        azureFunction.http (Handler(arrayQueryParam), Get, subRoute="/array")
        azureFunction.http (Handler(readonlyCollectionQueryParam), Get, subRoute="/readonlyCollection")
        azureFunction.http (Handler(listQueryParam), Get, subRoute="/list")
        azureFunction.http (Handler(listQueryStringParam), Get, subRoute="/stringList")
        azureFunction.http (Handler(enumerableQueryParam), Get, subRoute="/enumerable")
    ]
}
