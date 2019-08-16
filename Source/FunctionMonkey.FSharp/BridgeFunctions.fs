namespace FunctionMonkey.FSharp
open FunctionMonkey.Abstractions.Builders.Model
open System
open System.Threading.Tasks
open FunctionMonkey.Commanding.Abstractions.Validation
open Microsoft.AspNetCore.Mvc
open Models


module internal BridgeFunctions =
    let bridgeWith creator func =
        func |> Option.map(creator) |> Option.defaultValue null
    
    let createBridgedValidatorFunc (validator:'a -> ValidationError list) =
        new BridgedFunction(
            new System.Func<obj, FunctionMonkey.Commanding.Abstractions.Validation.ValidationResult>(fun cmd ->
                let createBridgedValidationError (error:FunctionMonkey.FSharp.Models.ValidationError) =
                    new FunctionMonkey.Commanding.Abstractions.Validation.ValidationError(
                        Severity = (match error.severity with
                                    | Error -> SeverityEnum.Error
                                    | Warning -> SeverityEnum.Warning
                                    | Info -> SeverityEnum.Info),
                        ErrorCode = (match error.errorCode with Some c -> c | None -> null),
                        Property = (match error.property with Some p -> p | None -> null),
                        Message = (match error.message with Some m -> m | None -> null)
                    )
                let result = validator (cmd :?> 'a)
                new FunctionMonkey.Commanding.Abstractions.Validation.ValidationResult(
                    Errors = (result |> Seq.map createBridgedValidationError |> Seq.toList)
                )
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
        
    let createBridgedValidationFailureResponseHandlerAsync (validationFailureResponseHandler:'a -> ValidationResult -> Async<IActionResult>) =
        new BridgedFunction(
            new System.Func<obj, ValidationResult, Task<IActionResult>>(
               fun cmd vr -> (validationFailureResponseHandler (cmd :?> 'a) vr) |> Async.StartAsTask
           )
        )

