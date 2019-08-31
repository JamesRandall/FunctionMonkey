namespace FmFsharpDemo
open System
open AccidentalFish.FSharp.Validation
open System.Security.Claims
open System.Web.Http
open FunctionMonkey.FSharp.Configuration
open FunctionMonkey.FSharp.Models
open Microsoft.AspNetCore.Mvc

module EntryPoint =
    exception InvalidTokenException
    exception AuthorizationException
    
    let validateToken (bearerToken:string) =
        match bearerToken.Length with
        | 0 -> raise InvalidTokenException
        | _ -> new ClaimsPrincipal(new ClaimsIdentity([new Claim("userId", "2FF4D861-F9E3-4694-9553-C49A94D7E665")]))
            
    let isResultValid result = match result with | Ok -> true | _ -> false
    
    let httpExceptionHandler _ (ex:Exception) =
        async {
            return match ex with
                   | :? AuthorizationException -> UnauthorizedResult() :> IActionResult
                   | _ -> InternalServerErrorResult() :> IActionResult
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
        // authorization
        defaultAuthorizationMode Token
        tokenValidator validateToken
        claimsMappings [
            claimsMapper.shared ("userId", "userId")
        ]
        // validation
        isValid isResultValid
        // functions
        httpRoute "version" [
            azureFunction.http (Handler(fun () -> "1.0.0"), Get, authorizationMode=Anonymous)
        ]
    }
                
