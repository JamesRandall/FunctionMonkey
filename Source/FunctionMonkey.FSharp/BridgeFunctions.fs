namespace FunctionMonkey.FSharp
open FunctionMonkey.Abstractions.Builders.Model
open System
open System.Threading.Tasks
open FunctionMonkey.Commanding.Abstractions.Validation
open Microsoft.AspNetCore.Mvc

module internal BridgeFunctions =
    let bridgeWith creator func =
        func |> Option.map(creator) |> Option.defaultValue null
    
    let createBridgedFunc (func:'a -> 'result) =
        new BridgedFunction(
            new System.Func<obj, 'result>(fun cmd ->
                let result = func (cmd :?> 'a)
                result
            )
        )
        
    let createBridgedExceptionResponseHandlerAsync (exceptionResponseHandler:'a -> Exception -> Async<IActionResult>) =
        new BridgedFunction(
            new System.Func<obj, Exception, Task<IActionResult>>(
                fun cmd e -> ((exceptionResponseHandler (cmd :?> 'a) e) |> Async.StartAsTask)
            )
        )
        
    let createBridgedResponseHandlerAsync (responseHandler:'a -> 'b -> Async<IActionResult>) =
        new BridgedFunction(
            new System.Func<obj, obj, Task<IActionResult>>(fun cmd res -> (responseHandler (cmd :?> 'a) (res :?> 'b) |> Async.StartAsTask))
        )
        
    let createBridgedValidationFailureResponseHandlerAsync (validationFailureResponseHandler:'a -> 'b -> Async<IActionResult>) =
        new BridgedFunction(
            new System.Func<obj, obj, Task<IActionResult>>(
               fun cmd vr -> (validationFailureResponseHandler (cmd :?> 'a) (vr :?> 'b)) |> Async.StartAsTask
           )
        )
        
    let createBridgedSerializer (func:obj -> bool -> string) =
        new BridgedFunction(
            new System.Func<obj, bool, string>(fun o e ->
                let result = func o e
                result
            )   
        )

    let createBridgedDeserializer (func:string -> bool -> obj) =
        new BridgedFunction(
            new System.Func<string, bool, obj>(fun o e ->
                let result = func o e
                result
            )   
        )