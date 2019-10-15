namespace FmFsharpDemo
open System
open AccidentalFish.FSharp.Validation
open FunctionMonkey.FSharp.OutputBindings
open System.Security.Claims
open System.Web.Http
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open Microsoft.AspNetCore.Mvc
open Newtonsoft.Json

module EntryPoint =
    exception InvalidTokenException
    exception AuthorizationException
    
    let validateToken (bearerToken:string) =
        match bearerToken.Length with
        | 0 -> raise InvalidTokenException
        | _ -> new ClaimsPrincipal(new ClaimsIdentity([new Claim("userIds", "2FF4D861-F9E3-4694-9553-C49A94D7E665")]))
            
    let isResultValid result = match result with | Ok -> true | _ -> false
    
    let httpExceptionHandler _ (ex:Exception) =
        async {
            return match ex with
                   | :? AuthorizationException -> UnauthorizedResult() :> IActionResult
                   | _ -> InternalServerErrorResult() :> IActionResult
        }
     
    type Version =
        | VersionSuccess of obj
        | Error of int
        
    let httpResponseHandlerFunc cmd result = async {
        return match result with
               | VersionSuccess v -> OkObjectResult(v) :> IActionResult
               | _ -> InternalServerErrorResult() :> IActionResult
    }

    type SbQueueCommand = {
        someValue: string
    }

    let app = functionApp {
        // diagnostics
        outputSourcePath "/Users/jamesrandall/code/authoredSource"
        outputOpenApiPath "/Users/jamesrandall/code/authoredSource"
        // open api
        openApi "ToDo" "1.0.0"
        openApiUserInterface "/openapi"
        // response handlers
        httpExceptionResponseHandler httpExceptionHandler
        httpResponseHandler httpResponseHandlerFunc
        // authorization
        defaultAuthorizationMode Token
        tokenValidator validateToken
        claimsMappings [
            claimsMapper.shared ("userId", "userId")
            claimsMapper.command("", (fun p -> p.someValue))
        ]
        defaultSerializer (fun o _ -> JsonConvert.SerializeObject(o))
        // validation
        isValid isResultValid
        // functions
        httpRoute "version" [
            azureFunction.http (Handler(fun () -> VersionSuccess(0,0,0)), Get, authorizationMode=Anonymous)
        ]
        httpRoute "queueItem" [
            azureFunction.http(FunctionHandler<SbQueueCommand, SbQueueCommand>.NoHandler,
                               Post,
                               authorizationMode=Anonymous,
                               serializer=(fun o _ -> JsonConvert.SerializeObject(o)))
                |> serviceBusQueue ("sbQueueCommand")
                |> withConnectionStringSettingName ""
        ]
        timers [
            azureFunction.timer(Handler(fun () -> System.Console.WriteLine("Timer func")), "*/5 * * * * *")
        ]
        (*serviceBus DefaultConnectionStringSettingName [
            azureFunction.serviceBusQueue (Handler(fun (c:SbQueueCommand) -> System.Console.WriteLine("SbQueueCommand: " + c.someValue)), "sbQueueCommand")
                |> serviceBusQueue ("junk")
        ]*)
    }
    
    (*
    let additionalFunctions = functions {
        serviceBus DefaultConnectionStringSettingName [
            azureFunction.serviceBusQueue (Handler(fun (c:SbQueueCommand) -> System.Console.WriteLine("SbQueueCommand: " + c.someValue) ; 50), "sbQueueCommand")
                //|> serviceBusQueue ("junk")
        ]
    }
    *)
    
                
